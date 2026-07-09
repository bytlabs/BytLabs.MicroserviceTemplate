import { Link } from 'react-router-dom';
import { ENTITIES } from '@/lib/entities';

export function Home() {
  return (
    <div className="container mx-auto py-8 space-y-4">
      <h1 className="text-3xl font-bold">Console</h1>
      <p className="text-muted-foreground">Manage entities and author their form/table schema.</p>
      <div className="grid gap-2 pt-4">
        {Object.keys(ENTITIES).map((t) => (
          <div key={t} className="flex gap-4">
            <Link className="text-primary hover:underline" to={`/entities/${t}`}>{t} records</Link>
            <Link className="text-muted-foreground hover:underline" to={`/schema/${t}`}>edit {t} schema</Link>
          </div>
        ))}
      </div>
    </div>
  );
}
