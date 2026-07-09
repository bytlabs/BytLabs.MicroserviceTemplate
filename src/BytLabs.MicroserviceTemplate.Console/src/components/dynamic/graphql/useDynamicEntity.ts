'use client';
import { DocumentNode, useMutation, useQuery } from '@apollo/client';

export interface DynamicEntityConfig {
  listQuery: DocumentNode;     // consumer's list query (e.g. GetProducts)
  createMutation: DocumentNode;
  updateMutation: DocumentNode;
  removeMutation: DocumentNode;
  listRoot: string;            // connection field name, e.g. 'products'
  variables?: Record<string, any>;
}

// Schema-agnostic wrapper so a console page can list/create/update/remove any flat entity by passing
// that entity's own operations. Pairs with useEntityDef (which supplies the render schema).
export function useDynamicEntity(config: DynamicEntityConfig) {
  const { data, loading, error, refetch } = useQuery(config.listQuery, { variables: config.variables });
  const [create] = useMutation(config.createMutation);
  const [update] = useMutation(config.updateMutation);
  const [remove] = useMutation(config.removeMutation);

  const connection = data?.[config.listRoot];
  return {
    nodes: connection?.nodes ?? [],
    totalCount: connection?.totalCount ?? 0,
    pageInfo: connection?.pageInfo,
    loading, error, refetch,
    create: (input: any) => create({ variables: { input } }).then((r) => { refetch(); return r; }),
    update: (input: any) => update({ variables: { input } }).then((r) => { refetch(); return r; }),
    remove: (input: any) => remove({ variables: { input } }).then((r) => { refetch(); return r; }),
  };
}
