import * as React from "react"
import {
  ColumnDef,
  SortingState,
  VisibilityState,
  flexRender,
  getCoreRowModel,
  useReactTable,
  ColumnSort,
} from "@tanstack/react-table"

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"

import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { ArrowDown, ArrowUp, Columns, ChevronLeft, ChevronRight } from "lucide-react"
import { DataOperationFilterInput, FilterOperation, InputMaybe, SortOrder, ValueKind } from "@/components/dynamic/graphql/types"
import { Skeleton } from "@/components/ui/skeleton"
import { useRef } from "react"
import FilterPopover, { FilterState, FilterCondition } from "./FilterPopover"

interface DataTableProps<TData, TValue, TSortInput extends { path: string, by?: InputMaybe<SortOrder> }, TFilterInput extends { and?: (InputMaybe<TFilterInput[]>) }> {
  columns: ColumnDef<TData, TValue>[]
  data: TData[]
  loading?: boolean
  onSortingChange?: (sorting: TSortInput[]) => void
  onFiltersChange?: (filters: TFilterInput) => void
  activeFilters?: TFilterInput
  activeSort?: TSortInput[]
  onEdit?: (data: TData) => void
  // Top-level scalar filter fields the API exposes directly (e.g. ["name"]). Anything not listed is
  // filtered via the generic dynamic `data.<path>` mechanism.
  knownColumns?: string[]
  // Column used by the quick-search box (must be one of knownColumns). Defaults to "name".
  searchColumn?: string
  pageInfo?: {
    hasNextPage: boolean
    hasPreviousPage: boolean
    startCursor?: string | null
    endCursor?: string | null
  }
  onPaginationChange?: (pagination: { cursor?: string; pageSize: number; direction: 'next' | 'prev' }) => void
  pageSize?: number
  onPageSizeChange?: (newPageSize: number) => void
  totalCount?: number
}

export function DataTable<TData, TValue, TSortInput extends { path: string, by?: InputMaybe<SortOrder> }, TFilterInput extends { and?: (InputMaybe<TFilterInput[]>), data?: InputMaybe<DataOperationFilterInput>; }>({
  columns,
  data,
  loading = false,
  onSortingChange,
  onFiltersChange,
  activeFilters,
  activeSort,
  knownColumns = [],
  searchColumn = "name",
  pageInfo,
  onPaginationChange,
  pageSize = 10,
  onPageSizeChange,
  totalCount = 0,
}: DataTableProps<TData, TValue, TSortInput, TFilterInput>) {

  const mapSortInput = (activeSort: TSortInput[]): ColumnSort[] => {
    if (!activeSort) return [];
    return activeSort.map(input => ({
      id: input.path.startsWith("data.") ? input.path.replace("data.", "") : input.path,
      desc: input.by == SortOrder.Desc
    } as ColumnSort))
  };

  const mapFilterInput = (filters: TFilterInput[]): { [key: string]: FilterState } => {
    const result: { [key: string]: FilterState } = {};

    const processFilters = (filters: TFilterInput[]) => {
      filters.forEach(filter => {
        if (filter.and && filter.and.length > 0) {
          processFilters(filter.and);
        }

        // Known top-level scalar fields (configurable) are read directly.
        knownColumns.forEach((key) => {
          if (key in (filter as any)) {
            const [operation, value] = Object.entries((filter as any)[key]!)[0] as [string, any];
            if (!result[key]) result[key] = { conditions: [] };
            result[key].conditions.push({
              operation: operation.toUpperCase() as FilterCondition["operation"],
              value,
            });
          }
        });

        if ("data" in filter) {
          const columnId = filter.data?.path || '';
          if (!result[columnId]) {
            result[columnId] = { conditions: [] };
          }
          result[columnId].conditions.push({
            operation: filter.data!.operation.toUpperCase() as FilterCondition["operation"],
            value: filter.data!.value,
          });
        }
      });
    };

    processFilters(filters);
    return result;
  };

  const [currentPage, setCurrentPage] = React.useState(1);
  const [searchInput, setSearchInput] = React.useState<string>('');
  const [sorting, setSorting] = React.useState<SortingState>(mapSortInput(activeSort ?? []) || [])
  const [columnVisibility, setColumnVisibility] = React.useState<VisibilityState>({ id: false })
  const [rowSelection, setRowSelection] = React.useState({})
  const [localFilters, setLocalFilters] = React.useState<{ [key: string]: FilterState }>(mapFilterInput(activeFilters?.and ?? []) || {})
  const sortingRef = useRef(sorting);
  React.useEffect(() => {
    if (sortingRef.current == sorting) return;

    if (onSortingChange) {
      const sortInput: TSortInput[] = sorting.map((sort) => ({
        path: sort.id.includes('.') ? `data.${sort.id}` : sort.id,
        by: sort.desc ? SortOrder.Desc : SortOrder.Asc
      }) as TSortInput)
      onSortingChange(sortInput)
    }
  }, [sorting])

  const handleFilterChange = (columnId: string, conditions: FilterCondition[]) => {
    setLocalFilters(prev => {
      const newFilters = { ...prev }

      if (conditions.length === 0) {
        delete newFilters[columnId]
      } else {
        newFilters[columnId] = { conditions }
      }

      setCurrentPage(1);

      const filterInputs = Object.keys(newFilters).map(columnId => {
        const conditions = newFilters[columnId].conditions;
        if (conditions.length === 0) return {} as TFilterInput

        if (knownColumns.includes(columnId)) {
          // Top-level scalar field: emit `{ <field>: { <op>: value } }`.
          const andConditions = conditions.map(condition => {
            const newCondition: any = {};
            newCondition[columnId] = {};
            newCondition[columnId][condition.operation.toLowerCase()] = condition.value
            return newCondition;
          })
          return { and: andConditions } as TFilterInput;
        } else {
          // Generic dynamic-data field.
          const andConditions = conditions.map(condition => ({
            data: {
              operation: condition.operation,
              path: columnId,
              value: condition.value,
              valueType: ValueKind.String,
            }
          })) as any[]
          return { and: andConditions } as TFilterInput
        }
      })

      onFiltersChange?.({ and: filterInputs } as TFilterInput)
      return newFilters
    })
  }

  const quickSearch = () => {
    setCurrentPage(1);
    handleFilterChange(searchColumn, searchInput ? [{
      value: searchInput,
      operation: FilterOperation.Contains
    }] : []);
  }

  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
    onSortingChange: setSorting,
    onColumnVisibilityChange: setColumnVisibility,
    onRowSelectionChange: setRowSelection,
    getColumnCanGlobalFilter: () => true,
    state: { sorting, columnVisibility, rowSelection },
    initialState: { sorting },
    manualSorting: true,
    sortDescFirst: true,
  })

  const getActiveFiltersForColumn = (columnId: string): FilterState | null => {
    const localFilter = localFilters[columnId]
    if (localFilter) return localFilter

    if (!activeFilters) return null

    const activeFilterObj = (activeFilters as any);
    if (knownColumns.includes(columnId) && activeFilterObj[columnId]?.contains) {
      return {
        conditions: [{
          value: activeFilterObj[columnId].contains,
          operation: FilterOperation.Contains
        }]
      }
    }

    if (activeFilters.and) {
      const conditions = activeFilters.and
        .filter(condition => condition.data?.path === columnId)
        .map(condition => ({
          value: condition.data!.value,
          operation: condition.data!.operation
        }))
      if (conditions.length > 0) return { conditions }
    } else if (activeFilters.data && activeFilters.data.path === columnId) {
      return {
        conditions: [{ value: activeFilters.data.value, operation: activeFilters.data.operation }]
      }
    }

    return null
  }

  const totalPages = Math.ceil(totalCount / pageSize);

  const TableSkeleton = ({ columns }: { columns: number }) => (
    <TableBody>
      {Array.from({ length: 5 }).map((_, rowIndex) => (
        <TableRow key={rowIndex}>
          {Array.from({ length: columns }).map((_, colIndex) => (
            <TableCell key={colIndex}>
              <Skeleton className="h-6 w-full" />
            </TableCell>
          ))}
        </TableRow>
      ))}
    </TableBody>
  )

  return (
    <div>
      <div className="flex justify-between py-4 gap-2">
        <Input
          placeholder="Search..."
          className="max-w-sm"
          onChange={(e) => setSearchInput(e.target.value)}
          value={searchInput}
          onKeyUp={(e) => {
            if (e.key === 'Enter' || searchInput.trim() === '') {
              quickSearch();
            }
          }}
        />
        <DropdownMenu>
          <DropdownMenuTrigger render={<Button variant="outline" />}>Columns <Columns /></DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            {table.getAllColumns().filter((column) => column.getCanHide()).map((column) => (
              <DropdownMenuCheckboxItem
                key={column.id}
                className="capitalize"
                checked={column.getIsVisible()}
                onCheckedChange={(value) => column.toggleVisibility(!!value)}
                onSelect={e => e.preventDefault()}
              >
                {column.columnDef.header?.toString()}
              </DropdownMenuCheckboxItem>
            ))}
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
      <div className="rounded-md border">
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => {
                  const activeFilter = getActiveFiltersForColumn(header.column.id)
                  return (
                    <TableHead key={header.id}>
                      <div className="flex items-center gap-2">
                        <div
                          className="flex items-center gap-2 cursor-pointer select-none"
                          onClick={header.column.getToggleSortingHandler()}
                        >
                          {{
                            asc: <ArrowUp className="h-4 w-4" />,
                            desc: <ArrowDown className="h-4 w-4" />,
                          }[header.column.getIsSorted() as string] ?? null}
                          {flexRender(header.column.columnDef.header, header.getContext())}
                        </div>
                        {header.column.getCanFilter() && (
                          <FilterPopover
                            column={header.column}
                            activeFilter={activeFilter}
                            onFilterChange={(conditions) => handleFilterChange(header.column.id, conditions)}
                          />
                        )}
                      </div>
                    </TableHead>
                  )
                })}
              </TableRow>
            ))}
          </TableHeader>
          {!loading ? (
            <TableBody>
              {table.getRowModel().rows?.length ? (
                table.getRowModel().rows.map((row) => (
                  <TableRow key={row.id} data-state={row.getIsSelected() && "selected"}>
                    {row.getVisibleCells().map((cell) => (
                      <TableCell key={cell.id}>
                        {flexRender(cell.column.columnDef.cell, cell.getContext())}
                      </TableCell>
                    ))}
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={columns.length} className="h-24 text-center">
                    No results.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          ) : (
            <TableSkeleton columns={columns.length} />
          )}
        </Table>
      </div>
      <div className="flex items-center justify-between space-x-2 py-4">
        <div className="flex items-center space-x-2">
          <select
            value={pageSize}
            onChange={e => {
              setCurrentPage(1);
              onPageSizeChange?.(Number(e.target.value));
            }}
            className="h-8 w-[70px] rounded-md border border-input bg-background px-2 py-1 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
          >
            {[10, 20, 30, 40, 50].map(size => (
              <option key={size} value={size}>{size}</option>
            ))}
          </select>
          <p className="text-sm text-muted-foreground">
            items per page | Page {currentPage} of {totalPages} | Total: {totalCount} items
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button
            variant="outline"
            size="sm"
            onClick={() => {
              const cursor = pageInfo?.startCursor ?? undefined;
              setCurrentPage(prev => Math.max(1, prev - 1));
              onPaginationChange?.({ direction: 'prev', cursor, pageSize });
            }}
            disabled={!pageInfo?.hasPreviousPage}
          >
            <ChevronLeft className="h-4 w-4" />
            Previous
          </Button>
          <Button
            variant="outline"
            size="sm"
            onClick={() => {
              const cursor = pageInfo?.endCursor ?? undefined;
              setCurrentPage(prev => Math.min(totalPages, prev + 1));
              onPaginationChange?.({ direction: 'next', cursor, pageSize });
            }}
            disabled={!pageInfo?.hasNextPage}
          >
            Next
            <ChevronRight className="h-4 w-4" />
          </Button>
        </div>
      </div>
    </div>
  )
}
