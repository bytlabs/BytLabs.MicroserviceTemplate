// Generic input mapping for the dynamic entities. Because the form schema nests dynamic fields
// under `data` and keeps outer fields at the top level, the RJSF `formData` already has the shape
// of the GraphQL mutation input — so these builders are essentially pass-through, only:
//   - generating the aggregate id (idField differs per entity: `id` vs `orderId`),
//   - stripping Apollo's `__typename` from nested objects (items/variants) before sending,
//   - injecting a row id for collections whose input type requires one (Order line items).

const newId = () =>
  typeof crypto !== 'undefined' && crypto.randomUUID ? crypto.randomUUID() : `${Date.now()}-${Math.round(Math.random() * 1e9)}`;

function stripTypename<T>(value: T): T {
  if (Array.isArray(value)) return value.map(stripTypename) as unknown as T;
  if (value && typeof value === 'object') {
    const out: Record<string, unknown> = {};
    for (const [k, v] of Object.entries(value as Record<string, unknown>)) {
      if (k !== '__typename') out[k] = stripTypename(v);
    }
    return out as unknown as T;
  }
  return value;
}

// For collections whose GraphQL input requires an `id` per row (e.g. OrderItemInput), give each
// row an id. Existing rows keep theirs (`...row` wins); new rows get a fresh one.
function withRowIds(formData: any, rowIdFields: string[]) {
  const fd = { ...(formData ?? {}) };
  for (const field of rowIdFields) {
    if (Array.isArray(fd[field])) fd[field] = fd[field].map((row: any) => ({ id: newId(), ...row }));
  }
  return fd;
}

export function buildCreateInput(idField: string, formData: any, rowIdFields: string[] = []) {
  return { [idField]: newId(), ...withRowIds(stripTypename(formData ?? {}), rowIdFields) };
}

export function buildUpdateInput(id: string, formData: any, rowIdFields: string[] = []) {
  return { id, ...withRowIds(stripTypename(formData ?? {}), rowIdFields) };
}

export function toRemoveInput(id: string) {
  return { id };
}

// Map a table row back to form data: pick only the fields the form schema declares (drops id,
// status, __typename, etc.), keeping the nested `data` bag and any outer collections intact.
export function rowToFormData(row: any, formSchema: any) {
  const keys = Object.keys(formSchema?.properties ?? {});
  const out: Record<string, unknown> = {};
  for (const key of keys) out[key] = stripTypename(row?.[key]);
  return out;
}
