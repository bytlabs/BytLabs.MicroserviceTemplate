'use client';
import { ApolloProvider } from '@apollo/client';
import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import { loadConfig, ConsoleConfig } from '@/lib/config';
import { makeApolloClient } from '@/lib/apollo';
import { GraphqlDataProvider, RestDataProvider } from '@/lib/data/context';
import { AuthGate } from './auth-gate';

const ConfigContext = createContext<ConsoleConfig | null>(null);
export const useConsoleConfig = () => useContext(ConfigContext);

// Global gate: loads runtime config from /console/config and enforces auth. Transport-agnostic — the
// per-mode data provider (GraphQL vs REST) is chosen by route below.
export function Providers({ children }: { children: React.ReactNode }) {
  const [config, setConfig] = useState<ConsoleConfig | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadConfig().then(setConfig).catch((e) => setError(String(e)));
  }, []);

  if (error) {
    return (
      <div className="p-8 text-sm text-muted-foreground">
        Cannot reach <code>/console/config</code>. Serve the console via the API (it is served under /console). {error}
      </div>
    );
  }
  if (!config) return <div className="p-8">Loading…</div>;

  return (
    <ConfigContext.Provider value={config}>
      <AuthGate config={config}>{children}</AuthGate>
    </ConfigContext.Provider>
  );
}

/** GraphQL mode shell: Apollo client (from config) + the GraphQL DataClient. */
export function GraphqlShell({ children }: { children: React.ReactNode }) {
  const config = useConsoleConfig()!;
  const client = useMemo(() => makeApolloClient(config.graphqlEndpoint), [config.graphqlEndpoint]);
  return (
    <ApolloProvider client={client}>
      <GraphqlDataProvider>{children}</GraphqlDataProvider>
    </ApolloProvider>
  );
}

/** REST/OData mode shell: the REST DataClient (no Apollo). */
export function RestShell({ children }: { children: React.ReactNode }) {
  return <RestDataProvider>{children}</RestDataProvider>;
}
