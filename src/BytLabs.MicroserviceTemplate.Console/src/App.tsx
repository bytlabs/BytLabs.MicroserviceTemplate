import { BrowserRouter, Routes, Route, Navigate, useParams } from 'react-router-dom';
import { Providers, GraphqlShell, RestShell } from './providers';
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

// Routes shared by both transports; mounted under /graphql and /rest with the matching data provider.
function ModeRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="entities/:type" element={<EntityRoute />} />
      <Route path="schema/:type" element={<SchemaRoute />} />
      <Route path="*" element={<Navigate to="." replace />} />
    </Routes>
  );
}

// The console runs the same schema-driven UI against either the GraphQL API (/console/graphql/*) or the
// REST/OData API (/console/rest/*); the mode is the first path segment and selects the data provider.
export function App() {
  return (
    <BrowserRouter basename="/console">
      <Providers>
        <ConsoleLayout>
          <Routes>
            <Route path="/graphql/*" element={<GraphqlShell><ModeRoutes /></GraphqlShell>} />
            <Route path="/rest/*" element={<RestShell><ModeRoutes /></RestShell>} />
            <Route path="*" element={<Navigate to="/graphql" replace />} />
          </Routes>
        </ConsoleLayout>
      </Providers>
    </BrowserRouter>
  );
}
