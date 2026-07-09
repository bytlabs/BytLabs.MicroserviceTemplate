import { ConsoleRouter } from '@/components/ConsoleRouter';
import { ENTITIES } from '@/lib/entities';

// Prerender the SPA shell PLUS every known entity route, so the static export contains each route's
// RSC payload. Without this, App Router client navigation to /entities/[type] or /schema/[type]
// requests a __next._tree.txt that doesn't exist and 404s. Adding a new entity type needs a rebuild.
export function generateStaticParams() {
  const params: { slug: string[] }[] = [{ slug: [] }];
  for (const type of Object.keys(ENTITIES)) {
    params.push({ slug: ['entities', type] });
    params.push({ slug: ['schema', type] });
  }
  return params;
}

export default function Page() {
  return <ConsoleRouter />;
}
