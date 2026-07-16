import { useLocation, useNavigate } from 'react-router-dom';
import { SidebarInset, SidebarProvider, SidebarTrigger } from '@/components/ui/sidebar';
import { Separator } from '@/components/ui/separator';
import { AppSidebar } from './app-sidebar';

// Toggles the transport the console talks to (GraphQL vs REST/OData), preserving the current sub-path.
function ModeSwitcher() {
  const { pathname } = useLocation();
  const navigate = useNavigate();
  const mode = pathname.split('/')[1] === 'rest' ? 'rest' : 'graphql';
  const sub = pathname.replace(/^\/(graphql|rest)/, '') || '/';
  const cls = (active: boolean) =>
    `rounded px-2 py-1 ${active ? 'bg-primary text-primary-foreground' : 'text-muted-foreground'}`;
  return (
    <div className="ml-auto flex items-center gap-0.5 rounded-md border p-0.5 text-xs">
      <button type="button" className={cls(mode === 'graphql')} onClick={() => navigate(`/graphql${sub}`)}>GraphQL</button>
      <button type="button" className={cls(mode === 'rest')} onClick={() => navigate(`/rest${sub}`)}>REST</button>
    </div>
  );
}

// sidebar-07-style shell: collapsible sidebar + an inset with a sticky header. Wraps the routed pages.
export function ConsoleLayout({ children }: { children: React.ReactNode }) {
  return (
    <SidebarProvider>
      <AppSidebar />
      <SidebarInset>
        <header className="flex h-14 shrink-0 items-center gap-2 border-b px-4">
          <SidebarTrigger className="-ml-1" />
          <Separator orientation="vertical" className="mr-2 h-4" />
          <span className="text-sm font-medium">BytLabs Console</span>
          <ModeSwitcher />
        </header>
        <div className="flex-1 overflow-auto">{children}</div>
      </SidebarInset>
    </SidebarProvider>
  );
}
