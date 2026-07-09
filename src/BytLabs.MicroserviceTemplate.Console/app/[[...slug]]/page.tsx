import { ConsoleRouter } from '@/components/ConsoleRouter';

// Single static shell for the SPA. Only the index is prerendered; deeper routes (/entities/[type],
// /schema/[type]) resolve client-side after the API serves index.html via its SPA fallback.
export function generateStaticParams() {
  return [{ slug: [] }];
}

export default function Page() {
  return <ConsoleRouter />;
}
