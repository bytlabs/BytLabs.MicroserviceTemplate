import { CellContext, ColumnDef } from "@tanstack/react-table"
import { Button } from "@/components/ui/button"
import { Checkbox } from "@/components/ui/checkbox"
import { MoreHorizontal } from "lucide-react"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { JSX } from "react"


export type ColumnAction<T> = (arg: T ) => JSX.Element;

interface ColumnProps<TData> {
  columns: DynamicColumnDef[]
  actions: ColumnAction<TData>[]
}

export interface DynamicColumnDef {
    id: string,
    accessorKey: string,
    header: string,
    enableSorting: boolean,
    enableColumnFilter: boolean
}

export function createColumns<TData>({ columns, actions }: ColumnProps<TData>, ): ColumnDef<TData>[] {
  return [
    {
      id: "select",
      header: ({ table }) => (
        <Checkbox
          checked={table.getIsAllPageRowsSelected()}
          onCheckedChange={(value) => table.toggleAllPageRowsSelected(!!value)}
          aria-label="Select all"
        />
      ),
      cell: ({ row }) => (
        <Checkbox
          checked={row.getIsSelected()}
          onCheckedChange={(value) => row.toggleSelected(!!value)}
          aria-label="Select row"
        />
      ),
      enableSorting: false,
      enableHiding: false,
    },
    {
      accessorKey: "id",
      header: "ID",
      cell: ({ row }) => (<div>{row.getValue<string>('id').slice(-6)}</div>),
      enableSorting: true,
    },
  
    ...columns,
  
    {
      id: "actions",
      header: "Actions",
      cell: ({ row }) => {
        return (
          <DropdownMenu>
            <DropdownMenuTrigger render={<Button variant="ghost" className="h-8 w-8 p-0" />}><span className="sr-only">Open menu</span><MoreHorizontal className="h-4 w-4" /></DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              {actions.map(action=> action(row.original))}
            </DropdownMenuContent>
          </DropdownMenu>
        )
      },    
      enableSorting: false,
      enableColumnFilter: false,
      enableHiding: false
    },
  ] 

} 