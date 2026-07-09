export interface ConsoleConfig {
  authMode: 'Basic' | 'Oidc' | 'None';
  oidc: { authority: string; clientId: string };
  graphqlEndpoint: string;
}

// Fetched from the .NET API at runtime (served alongside the static console). No rebuild needed to
// change auth mode / endpoint.
export async function loadConfig(): Promise<ConsoleConfig> {
  const res = await fetch('/console/config', { cache: 'no-store' });
  if (!res.ok) throw new Error(`Failed to load /console/config (${res.status})`);
  return res.json();
}

const TOKEN_KEY = 'console_token';

export function getToken(): string | null {
  return typeof window !== 'undefined' ? window.localStorage.getItem(TOKEN_KEY) : null;
}

export function setToken(token: string) {
  window.localStorage.setItem(TOKEN_KEY, token);
}

export function clearToken() {
  window.localStorage.removeItem(TOKEN_KEY);
}
