import { ChevronsUpDown, LogOut } from 'lucide-react';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import {
  SidebarMenu,
  SidebarMenuItem,
  sidebarMenuButtonVariants,
  useSidebar,
} from '@/components/ui/sidebar';
import { cn } from '@/lib/utils';
import { clearToken } from '@/lib/config';

function logout() {
  clearToken();
  // Full reload so Providers re-fetches config and AuthGate re-evaluates -> shows the login form.
  window.location.assign('/console/');
}

export function NavUser() {
  const { isMobile } = useSidebar();

  const user = (
    <>
      <Avatar className="h-8 w-8 rounded-lg">
        <AvatarFallback className="rounded-lg">AD</AvatarFallback>
      </Avatar>
      <div className="grid flex-1 text-left text-sm leading-tight">
        <span className="truncate font-semibold">Admin</span>
        <span className="truncate text-xs">Console</span>
      </div>
    </>
  );

  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <DropdownMenu>
          {/* Style the trigger's own button rather than nesting SidebarMenuButton via `render`:
              chaining two Base UI `useRender` components (Trigger + SidebarMenuButton) throws
              Base UI error #31. A single `useRender` (the Trigger) rendering a plain styled button works. */}
          <DropdownMenuTrigger
            className={cn(
              sidebarMenuButtonVariants({ size: 'lg' }),
              'data-[popup-open]:bg-sidebar-accent data-[popup-open]:text-sidebar-accent-foreground',
            )}
          >
            {user}
            <ChevronsUpDown className="ml-auto size-4" />
          </DropdownMenuTrigger>
          <DropdownMenuContent
            className="min-w-56 rounded-lg"
            side={isMobile ? 'bottom' : 'right'}
            align="end"
            sideOffset={4}
          >
            {/* GroupLabel (base-nova's DropdownMenuLabel) requires a Menu.Group ancestor — omitting
                the DropdownMenuGroup wrapper throws Base UI error #31 and aborts the whole menu. */}
            <DropdownMenuGroup>
              <DropdownMenuLabel className="p-0 font-normal">
                <div className="flex items-center gap-2 px-1 py-1.5 text-left text-sm">{user}</div>
              </DropdownMenuLabel>
            </DropdownMenuGroup>
            <DropdownMenuSeparator />
            <DropdownMenuItem onClick={logout}>
              <LogOut />
              Log out
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </SidebarMenuItem>
    </SidebarMenu>
  );
}
