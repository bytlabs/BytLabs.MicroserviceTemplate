import { useEffect, useState } from 'react';
import { getToken } from '@/lib/config';
import { ENTITIES } from '@/lib/entities';
import type { DataClient, EntityDefState, EntityListState } from './DataClient';

// REST/OData implementation of the console DataClient. Reconciles OData shapes with what the UI expects:
// PascalCase -> camelCase, the `data` jsonb string <-> object, and OData offset paging <-> the DataTable's
// cursor pageInfo. Reuses the same schema-driven UI as the GraphQL path.

const odataSet = (entityType: string) => `${entityType}s`; // Product -> Products, Order -> Orders, EntityDef -> EntityDefs

function headers(): HeadersInit {
  const token = getToken();
  return {
    'Content-Type': 'application/json',
    Tenant: 'default',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };
}

async function odataFetch(path: string, init?: RequestInit): Promise<any> {
  const res = await fetch(`/odata${path}`, { ...init, headers: { ...headers(), ...(init?.headers ?? {}) } });
  if (!res.ok) throw new Error(`OData ${init?.method ?? 'GET'} ${path} failed (${res.status})`);
  const text = await res.text();
  return text ? JSON.parse(text) : null;
}

const lower = (s: string) => (s ? s[0].toLowerCase() + s.slice(1) : s);
const upper = (s: string) => (s ? s[0].toUpperCase() + s.slice(1) : s);
const JSON_FIELDS = new Set(['data']);

// OData entity -> UI node: camelCase keys, recurse collections (items/variants), and parse the top-level
// dynamic `data` jsonb string into an object (its user-defined keys are left untouched).
function toNode(entity: any): any {
  if (Array.isArray(entity)) return entity.map(toNode);
  if (!entity || typeof entity !== 'object') return entity;
  const out: Record<string, any> = {};
  for (const [k, v] of Object.entries(entity)) {
    if (k.startsWith('@')) continue;
    const key = lower(k);
    out[key] = JSON_FIELDS.has(key) && typeof v === 'string' ? safeParse(v) : toNode(v);
  }
  return out;
}

// OData EntityDefResource (Form/Table as JSON strings of the value objects) -> the nested camelCase shape
// the UI expects, keeping each innermost `.data` as a string (the schema editor parses those itself).
function toEntityDef(row: any): any {
  const form = safeParse(row.Form ?? '{}');
  const table = safeParse(row.Table ?? '{}');
  const ds = (o: any) => (o ? { type: o.Type, data: o.Data } : undefined);
  return {
    id: row.Id,
    entityType: row.EntityType,
    form: { key: form.Key, formSchema: ds(form.FormSchema), formUi: ds(form.FormUi), sampleData: ds(form.SampleData) },
    table: { columns: ds(table.Columns), filter: ds(table.Filter), sampleData: ds(table.SampleData), details: ds(table.Details) },
  };
}

// UI input (camelCase, `data` as object) -> OData payload (PascalCase, `data` as JSON string).
function toPayload(input: any): any {
  if (Array.isArray(input)) return input.map(toPayload);
  if (!input || typeof input !== 'object') return input;
  const out: Record<string, any> = {};
  for (const [k, v] of Object.entries(input)) {
    if (k === '__typename') continue;
    const key = upper(k === 'orderId' ? 'id' : k); // aggregates expose Id on the REST resource
    out[key] = JSON_FIELDS.has(k.toLowerCase()) ? JSON.stringify(v ?? {}) : toPayload(v);
  }
  return out;
}

function safeParse(raw: string): any {
  try { return JSON.parse(raw); } catch { return raw; }
}

const encodeCursor = (skip: number) => btoa(String(skip));
const decodeCursor = (c?: string) => (c ? Number(atob(c)) : 0);

export function createRestClient(): DataClient {
  return {
    mode: 'rest',

    useEntity(entityType: string, vars: Record<string, any>): EntityListState {
      const set = odataSet(entityType);
      const [state, setState] = useState<Omit<EntityListState, 'refetch' | 'create' | 'update' | 'remove'>>({
        nodes: [], totalCount: 0, pageInfo: null, loading: true, error: null,
      });
      const [tick, setTick] = useState(0);
      const varsKey = JSON.stringify(vars);

      useEffect(() => {
        let cancelled = false;
        setState((s) => ({ ...s, loading: true }));
        const top = vars.first ?? vars.last ?? 10;
        const skip = decodeCursor(vars.after);
        odataFetch(`/${set}?$count=true&$top=${top}&$skip=${skip}`)
          .then((json) => {
            if (cancelled) return;
            const totalCount = json?.['@odata.count'] ?? 0;
            setState({
              nodes: (json?.value ?? []).map(toNode),
              totalCount,
              pageInfo: {
                hasNextPage: skip + top < totalCount,
                hasPreviousPage: skip > 0,
                startCursor: encodeCursor(skip),
                endCursor: encodeCursor(skip + top),
              },
              loading: false,
              error: null,
            });
          })
          .catch((error) => !cancelled && setState((s) => ({ ...s, loading: false, error })));
        return () => { cancelled = true; };
      }, [set, varsKey, tick]);

      const refetch = () => setTick((t) => t + 1);
      const idOf = (input: any) => input.id ?? input.orderId;
      return {
        ...state,
        refetch,
        create: async (input) => { await odataFetch(`/${set}`, { method: 'POST', body: JSON.stringify(toPayload(input)) }); refetch(); },
        update: async (input) => { await odataFetch(`/${set}(${idOf(input)})`, { method: 'PUT', body: JSON.stringify(toPayload(input)) }); refetch(); },
        remove: async (input) => { await odataFetch(`/${set}(${input.id})`, { method: 'DELETE' }); refetch(); },
      };
    },

    useEntityDef(entityType: string): EntityDefState {
      const [def, setDef] = useState<any>(undefined);
      const [loading, setLoading] = useState(true);
      const [tick, setTick] = useState(0);
      const refetch = () => setTick((t) => t + 1);

      useEffect(() => {
        let cancelled = false;
        setLoading(true);
        odataFetch(`/EntityDefs?$filter=EntityType eq '${entityType}'&$top=1`)
          .then((json) => {
            if (cancelled) return;
            const row = (json?.value ?? [])[0];
            setDef(row ? toEntityDef(row) : undefined);
            setLoading(false);
          })
          .catch(() => !cancelled && setLoading(false));
        return () => { cancelled = true; };
      }, [entityType, tick]);

      const toResource = (data: any) => ({
        EntityType: entityType,
        Form: JSON.stringify({
          Key: entityType,
          SampleData: { Type: 'json', Data: JSON.stringify(data.form.sampleData ?? {}) },
          FormSchema: { Type: 'rjsf/formSchema', Data: JSON.stringify(data.form.formSchema ?? {}) },
          FormUi: { Type: 'rjsf/uiSchema', Data: JSON.stringify(data.form.uiSchema ?? {}) },
        }),
        Table: JSON.stringify({
          SampleData: { Type: 'json', Data: JSON.stringify(data.table.sampleData ?? {}) },
          Columns: { Type: 'tanstack/columnDef', Data: JSON.stringify(data.table.columns ?? []) },
          Filter: { Type: 'json', Data: JSON.stringify(data.table.filter ?? {}) },
          Details: { Type: 'cms/view', Data: JSON.stringify(data.table.details ?? {}) },
        }),
      });

      return {
        def, loading, refetch,
        handleCreate: async (data) => { await odataFetch('/EntityDefs', { method: 'POST', body: JSON.stringify({ Id: crypto.randomUUID(), ...toResource(data) }) }); refetch(); },
        handleUpdate: async (id, data) => { await odataFetch(`/EntityDefs(${id})`, { method: 'PUT', body: JSON.stringify(toResource(data)) }); refetch(); },
        handleDelete: async (id) => { await odataFetch(`/EntityDefs(${id})`, { method: 'DELETE' }); refetch(); },
      };
    },

    async loadOptions(targetType: string) {
      const target = ENTITIES[targetType];
      if (!target) return [];
      const json = await odataFetch(`/${odataSet(targetType)}?$top=50`);
      return (json?.value ?? []).map(toNode).map((n: any) => ({ value: n.id, label: String(n[target.labelField] ?? n.id) }));
    },
  };
}
