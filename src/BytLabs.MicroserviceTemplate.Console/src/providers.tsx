'use client';
import { ApolloProvider } from '@apollo/client';
import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import { loadConfig, ConsoleConfig } from '@/lib/config';
import { makeApolloClient } from '@/lib/apollo';
import { AuthGate } from './auth-gate';

const ConfigContext = createContext<ConsoleConfig | null>(null);
export const useConsoleConfig = () => useContext(ConfigContext);

export function Providers({ children }: { children: React.ReactNode }) {
  const [config, setConfig] = useState<ConsoleConfig | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadConfig().then(setConfig).catch((e) => setError(String(e)));
  }, []);

  const client = useMemo(() => (config ? makeApolloClient(config.graphqlEndpoint) : null), [config]);

  if (error) {
    return (
      <div className="p-8 text-sm text-muted-foreground">
        Cannot reach <code>/console/config</code>. Serve the console via the API (it is a static export served under /console). {error}
      </div>
    );
  }
  if (!client || !config) return <div className="p-8">Loading…</div>;

  return (
    <ConfigContext.Provider value={config}>
      <ApolloProvider client={client}>
        <AuthGate config={config}>{children}</AuthGate>
      </ApolloProvider>
    </ConfigContext.Provider>
  );
}
