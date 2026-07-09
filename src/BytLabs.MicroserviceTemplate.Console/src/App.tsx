import { BrowserRouter, Routes, Route, Navigate, useParams } from 'react-router-dom';
import { Providers } from './providers';
import { Home } from './home';
import { ConsoleLayout } from '@/components/console-layout';
import { EntityManager } from '@/components/EntityManager';
import { SchemaAuthoring } from '@/components/SchemaAuthoring';

function EntityRoute() {
  const { type } = useParams();
  return <EntityManager entityType={type!} />;
}

function SchemaRoute() {
  const { type } = useParams();
  return <SchemaAuthoring entityType={type!} />;
}

// Client-side routing. `basename` matches the /console path the API serves the SPA under, so links
// like /entities/Product resolve to /console/entities/Product with no per-route build (unlike the
// previous Next static export).
export function App() {
  return (
    <BrowserRouter basename="/console">
      <Providers>
        <ConsoleLayout>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/entities/:type" element={<EntityRoute />} />
            <Route path="/schema/:type" element={<SchemaRoute />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </ConsoleLayout>
      </Providers>
    </BrowserRouter>
  );
}
