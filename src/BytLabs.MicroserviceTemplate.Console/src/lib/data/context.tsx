'use client';
import { createContext, useContext, useMemo } from 'react';
import { ApolloClient, useApolloClient } from '@apollo/client';
import { ENTITIES } from '@/lib/entities';
import { useDynamicEntity } from '@/components/dynamic/graphql/useDynamicEntity';
import { useEntityDef } from '@/components/dynamic/graphql/useEntityDef';
import type { DataClient } from './DataClient';
import { createRestClient } from './restClient';

// GraphQL DataClient: wraps the existing Apollo-based hooks + per-entity operations.
function makeGraphqlClient(apollo: ApolloClient<object>): DataClient {
  return {
    mode: 'graphql',
    useEntity(entityType, vars) {
      const reg = ENTITIES[entityType];
      return useDynamicEntity(
        reg
          ? { listQuery: reg.listQuery, createMutation: reg.createMutation, updateMutation: reg.updateMutation, removeMutation: reg.removeMutation, listRoot: reg.listRoot, variables: vars }
          : ({ listQuery: { kind: 'Document', definitions: [] } as any, createMutation: {} as any, updateMutation: {} as any, removeMutation: {} as any, listRoot: '', variables: {} })
      );
    },
    useEntityDef(entityType) {
      return useEntityDef(entityType);
    },
    async loadOptions(targetType) {
      const target = ENTITIES[targetType];
      if (!target) return [];
      const { data } = await apollo.query({ query: target.listQuery, variables: { first: 50 }, fetchPolicy: 'cache-first' });
      const nodes = (data as any)?.[target.listRoot]?.nodes ?? [];
      return nodes.map((n: any) => ({ value: n.id, label: String(n[target.labelField] ?? n.id) }));
    },
  };
}

const DataClientContext = createContext<DataClient | null>(null);

export function useDataClient(): DataClient {
  const client = useContext(DataClientContext);
  if (!client) throw new Error('useDataClient must be used within a Graphql/Rest data provider.');
  return client;
}

/** GraphQL mode: an ApolloProvider must be above this. */
export function GraphqlDataProvider({ children }: { children: React.ReactNode }) {
  const apollo = useApolloClient();
  const client = useMemo(() => makeGraphqlClient(apollo), [apollo]);
  return <DataClientContext.Provider value={client}>{children}</DataClientContext.Provider>;
}

/** REST/OData mode: no ApolloProvider required. */
export function RestDataProvider({ children }: { children: React.ReactNode }) {
  const client = useMemo(() => createRestClient(), []);
  return <DataClientContext.Provider value={client}>{children}</DataClientContext.Provider>;
}
