import { DataTable } from "@/components/dynamic/DataTable";
import { DynamicColumnDef } from "@/components/dynamic/CreateColumns";
import { useState } from "react";
import { SortOrder } from "@/components/dynamic/graphql/types";
import { ViewEntity, ViewSchema } from "@/components/dynamic/ViewEntity";
import { ColumnDef } from "@tanstack/react-table";

interface DataTablePreviewProps {
  columns: DynamicColumnDef[];
  sampleData: any[];
  details: ViewSchema;
}

export function DataTablePreview({ columns, sampleData, details }: DataTablePreviewProps) {
  const [sorting, setSorting] = useState<{ path: string; by?: SortOrder }[]>([]);
  const [filters, setFilters] = useState<any>({});
  const [selectedItem, setSelectedItem] = useState<any | null>(null);

  // Add action column for view details
  const columnsWithActions: ColumnDef<any, any>[] = [
    ...columns,
    {
      id: 'actions',
      header: 'Actions',
      cell: ({ row }) => (
        <button
          onClick={() => setSelectedItem(row.original)}
          className="text-primary hover:underline"
        >
          View
        </button>
      ),
    },
  ];

  return (
    <>
      <DataTable
        columns={columnsWithActions}
        data={sampleData}
        onSortingChange={setSorting}
        onFiltersChange={setFilters}
        activeFilters={filters}
        activeSort={sorting}
        pageInfo={{
          hasNextPage: false,
          hasPreviousPage: false,
        }}
      />

      {selectedItem && (
        <div className="mt-4 border rounded-md p-4">
          <button
            onClick={() => setSelectedItem(null)}
            className="text-sm text-muted-foreground hover:underline mb-2"
          >
            Close
          </button>
          <ViewEntity data={selectedItem} schema={details} />
        </div>
      )}
    </>
  );
} 