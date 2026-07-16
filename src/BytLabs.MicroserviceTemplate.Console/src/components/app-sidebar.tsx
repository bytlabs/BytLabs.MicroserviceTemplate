import { Link, useLocation } from 'react-router-dom';
import { Boxes, FileCog, Home, Table2 } from 'lucide-react';
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarRail,
} from '@/components/ui/sidebar';
import { ENTITIES } from '@/lib/entities';
import { NavUser } from './nav-user';

export function AppSidebar() {
  const { pathname } = useLocation();
  const types = Object.keys(ENTITIES);
  // Current transport mode from the first path segment; links are prefixed so nav stays within it.
  const mode = pathname.split('/')[1] === 'rest' ? 'rest' : 'graphql';
  const home = `/${mode}`;
  const entityPath = (t: string) => `/${mode}/entities/${t}`;
  const schemaPath = (t: string) => `/${mode}/schema/${t}`;

  return (
    <Sidebar collapsible="icon">
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton size="lg" render={<Link to={home} />}>
              <div className="flex aspect-square size-8 items-center justify-center rounded-lg bg-sidebar-primary text-sidebar-primary-foreground">
                <Boxes className="size-4" />
              </div>
              <div className="grid flex-1 text-left text-sm leading-tight">
                <span className="truncate font-semibold">BytLabs</span>
                <span className="truncate text-xs">Console</span>
              </div>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>

      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel>Platform</SidebarGroupLabel>
          <SidebarMenu>
            <SidebarMenuItem>
              <SidebarMenuButton isActive={pathname === home} tooltip="Home" render={<Link to={home} />}>
                <Home />
                <span>Home</span>
              </SidebarMenuButton>
            </SidebarMenuItem>
          </SidebarMenu>
        </SidebarGroup>

        <SidebarGroup>
          <SidebarGroupLabel>Records</SidebarGroupLabel>
          <SidebarMenu>
            {types.map((t) => (
              <SidebarMenuItem key={t}>
                <SidebarMenuButton
                  isActive={pathname === entityPath(t)}
                  tooltip={t}
                  render={<Link to={entityPath(t)} />}
                >
                  <Table2 />
                  <span>{t}</span>
                </SidebarMenuButton>
              </SidebarMenuItem>
            ))}
          </SidebarMenu>
        </SidebarGroup>

        <SidebarGroup>
          <SidebarGroupLabel>Schema</SidebarGroupLabel>
          <SidebarMenu>
            {types.map((t) => (
              <SidebarMenuItem key={t}>
                <SidebarMenuButton
                  isActive={pathname === schemaPath(t)}
                  tooltip={`${t} schema`}
                  render={<Link to={schemaPath(t)} />}
                >
                  <FileCog />
                  <span>{t}</span>
                </SidebarMenuButton>
              </SidebarMenuItem>
            ))}
          </SidebarMenu>
        </SidebarGroup>
      </SidebarContent>

      <SidebarFooter>
        <NavUser />
      </SidebarFooter>
      <SidebarRail />
    </Sidebar>
  );
}
