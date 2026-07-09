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

  return (
    <Sidebar collapsible="icon">
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton size="lg" render={<Link to="/" />}>
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
              <SidebarMenuButton isActive={pathname === '/'} tooltip="Home" render={<Link to="/" />}>
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
                  isActive={pathname === `/entities/${t}`}
                  tooltip={t}
                  render={<Link to={`/entities/${t}`} />}
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
                  isActive={pathname === `/schema/${t}`}
                  tooltip={`${t} schema`}
                  render={<Link to={`/schema/${t}`} />}
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
