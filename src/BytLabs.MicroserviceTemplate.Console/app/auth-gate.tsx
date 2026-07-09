'use client';
import { useEffect, useState } from 'react';
import { ConsoleConfig, getToken } from '@/lib/config';
import { LoginForm } from './login-form';

export function AuthGate({ config, children }: { config: ConsoleConfig; children: React.ReactNode }) {
  const [authed, setAuthed] = useState<boolean>(config.authMode === 'None');

  useEffect(() => {
    if (config.authMode === 'None') { setAuthed(true); return; }
    setAuthed(!!getToken());
  }, [config]);

  if (config.authMode === 'Basic' && !authed) {
    return <LoginForm onSuccess={() => setAuthed(true)} />;
  }
  if (config.authMode === 'Oidc' && !authed) {
    return (
      <div className="p-8 text-sm">
        OIDC mode is configured (authority: <code>{config.oidc.authority || 'unset'}</code>). Wire an
        oidc-client-ts <code>UserManager</code> to complete sign-in, storing the access token as
        <code> console_token</code>.
      </div>
    );
  }
  return <>{children}</>;
}
