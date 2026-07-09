'use client';
import { usePathname } from 'next/navigation';
import Link from 'next/link';
import { EntityManager } from './EntityManager';
import { SchemaAuthoring } from './SchemaAuthoring';
import { ENTITIES } from '@/lib/entities';

// Client-side router for the static-export SPA. `usePathname()` returns the path without the /console
// basePath (Next strips it) and updates on client navigation. Deep links are served index.html by the
// API's SPA fallback, then resolved here.
export function ConsoleRouter() {
  const pathname = usePathname() || '/';
  const seg = pathname.split('/').filter(Boolean); // ['entities','Product'] etc.

  if (seg[0] === 'entities' && seg[1]) return <EntityManager entityType={decodeURIComponent(seg[1])} />;
  if (seg[0] === 'schema' && seg[1]) return <SchemaAuthoring entityType={decodeURIComponent(seg[1])} />;

  return (
    <div className="container mx-auto py-8 space-y-4">
      <h1 className="text-3xl font-bold">Console</h1>
      <p className="text-muted-foreground">Manage entities and author their form/table schema.</p>
      <div className="grid gap-2 pt-4">
        {Object.keys(ENTITIES).map((t) => (
          <div key={t} className="flex gap-4">
            <Link className="text-primary hover:underline" href={`/entities/${t}`}>{t} records</Link>
            <Link className="text-muted-foreground hover:underline" href={`/schema/${t}`}>edit {t} schema</Link>
          </div>
        ))}
      </div>
    </div>
  );
}
