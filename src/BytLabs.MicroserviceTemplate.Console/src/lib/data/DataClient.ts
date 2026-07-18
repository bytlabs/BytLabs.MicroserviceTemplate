// A store-/transport-agnostic data client the console pages use, so the same UI runs against either the
// GraphQL API (/graphql) or the REST/OData API (/odata). Selected by the route mode ('graphql' | 'rest').

export type DataMode = 'graphql' | 'rest';

export interface EntityListState {
  nodes: any[];
  totalCount: number;
  pageInfo: any;
  loading: boolean;
  error: unknown;
  refetch: () => void;
  create: (input: any) => Promise<any>;
  update: (input: any) => Promise<any>;
  remove: (input: any) => Promise<any>;
}

export interface EntityDefState {
  def: any | undefined;
  loading: boolean;
  refetch: () => void;
  handleCreate: (data: any) => Promise<any>;
  handleUpdate: (id: string, data: any) => Promise<any>;
  handleDelete: (id: string) => Promise<any>;
}

export interface DataClient {
  readonly mode: DataMode;
  /** Reactive list + create/update/remove for a flat entity. Hook — call once per render. */
  useEntity(entityType: string, vars: Record<string, any>): EntityListState;
  /** Reactive read + authoring of the EntityDef (render schema) for an entity. Hook. */
  useEntityDef(entityType: string): EntityDefState;
  /** Reference-picker options: { value: id, label } for a target entity. */
  loadOptions(targetType: string): Promise<{ value: string; label: string }[]>;
}
