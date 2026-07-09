'use client';
import { useMemo, useState } from 'react';
import { useEntityDef } from '@/components/dynamic/graphql/useEntityDef';
import { useDynamicEntity } from '@/components/dynamic/graphql/useDynamicEntity';
import { DataTable } from '@/components/dynamic/DataTable';
import { createColumns, ColumnAction } from '@/components/dynamic/CreateColumns';
import { DynamicForm } from '@/components/dynamic/DynamicForm';
import { ViewEntity } from '@/components/dynamic/ViewEntity';
import { Button } from '@/components/ui/button';
import { Sheet, SheetContent, SheetHeader, SheetTitle } from '@/components/ui/sheet';
import { DropdownMenuItem } from '@/components/ui/dropdown-menu';
import { ENTITIES } from '@/lib/entities';

function parse<T>(raw: string | undefined, fallback: T): T {
  if (!raw) return fallback;
  try { return JSON.parse(raw); } catch { return fallback; }
}

export function EntityManager({ entityType }: { entityType: string }) {
  const reg = ENTITIES[entityType];
  const { def, loading: defLoading } = useEntityDef(entityType);

  const [createOpen, setCreateOpen] = useState(false);
  const [editRow, setEditRow] = useState<any | null>(null);
  const [viewRow, setViewRow] = useState<any | null>(null);
  const [vars, setVars] = useState<Record<string, any>>({ first: 10 });

  const entity = useDynamicEntity(
    reg
      ? { listQuery: reg.listQuery, createMutation: reg.createMutation, updateMutation: reg.updateMutation, removeMutation: reg.removeMutation, listRoot: reg.listRoot, variables: vars }
      : { listQuery: { kind: 'Document', definitions: [] } as any, createMutation: {} as any, updateMutation: {} as any, removeMutation: {} as any, listRoot: '', variables: {} }
  );

  const formSchema = useMemo(() => parse<any>(def?.form?.formSchema?.data, { type: 'object' }), [def]);
  const uiSchema = useMemo(() => parse<any>(def?.form?.formUi?.data, {}), [def]);
  const columnDefs = useMemo(() => parse<any[]>(def?.table?.columns?.data, []), [def]);
  const viewSchema = useMemo(() => parse<any>(def?.table?.details?.data, { type: 'container', content: [] }), [def]);

  const actions: ColumnAction<any>[] = [
    (row) => <DropdownMenuItem key="view" onClick={() => setViewRow(row)}>View</DropdownMenuItem>,
    (row) => <DropdownMenuItem key="edit" onClick={() => setEditRow(row)}>Edit</DropdownMenuItem>,
    (row) => <DropdownMenuItem key="delete" onClick={() => reg && entity.remove(reg.toRemoveInput(row.id))}>Delete</DropdownMenuItem>,
  ];

  const columns = useMemo(() => createColumns({ columns: columnDefs, actions }), [columnDefs]);

  if (!reg) return <div className="p-8">Unknown entity type: <code>{entityType}</code>. Register it in <code>lib/entities.ts</code>.</div>;

  return (
    <div className="container mx-auto py-8 space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">{entityType}</h1>
        <Button onClick={() => setCreateOpen(true)}>Create {entityType}</Button>
      </div>

      {defLoading ? (
        <div>Loading definition…</div>
      ) : (
        <DataTable
          columns={columns as any}
          data={entity.nodes}
          loading={entity.loading}
          knownColumns={reg.knownColumns}
          totalCount={entity.totalCount}
          pageInfo={entity.pageInfo}
          onSortingChange={(order) => setVars((v) => ({ ...v, order }))}
          onFiltersChange={(where) => setVars((v) => ({ first: v.first ?? 10, where }))}
          onPaginationChange={({ direction, cursor, pageSize }) =>
            setVars((v) => direction === 'next'
              ? { ...v, first: pageSize, after: cursor, last: undefined, before: undefined }
              : { ...v, last: pageSize, before: cursor, first: undefined, after: undefined })
          }
          onPageSizeChange={(pageSize) => setVars((v) => ({ ...v, first: pageSize, after: undefined, last: undefined, before: undefined }))}
        />
      )}

      {/* Create */}
      <Sheet open={createOpen} onOpenChange={setCreateOpen}>
        <SheetContent side="right" className="w-[50%] sm:max-w-[50%] overflow-y-auto">
          <SheetHeader><SheetTitle>Create {entityType}</SheetTitle></SheetHeader>
          <div className="p-4">
            <DynamicForm
              schema={formSchema}
              uiSchema={uiSchema}
              className="space-y-4"
              onSubmit={async ({ formData }) => { await entity.create(reg.toCreateInput(formData)); setCreateOpen(false); }}
            >
              <Button type="submit">Save</Button>
            </DynamicForm>
          </div>
        </SheetContent>
      </Sheet>

      {/* Edit */}
      <Sheet open={!!editRow} onOpenChange={(o) => !o && setEditRow(null)}>
        <SheetContent side="right" className="w-[50%] sm:max-w-[50%] overflow-y-auto">
          <SheetHeader><SheetTitle>Edit {entityType}</SheetTitle></SheetHeader>
          <div className="p-4">
            {editRow && (
              <DynamicForm
                schema={formSchema}
                uiSchema={uiSchema}
                className="space-y-4"
                formData={reg.rowToFormData(editRow)}
                onSubmit={async ({ formData }) => { await entity.update(reg.toUpdateInput(editRow.id, formData)); setEditRow(null); }}
              >
                <Button type="submit">Save</Button>
              </DynamicForm>
            )}
          </div>
        </SheetContent>
      </Sheet>

      {/* View */}
      <Sheet open={!!viewRow} onOpenChange={(o) => !o && setViewRow(null)}>
        <SheetContent side="right" className="w-[50%] sm:max-w-[50%] overflow-y-auto">
          <SheetHeader><SheetTitle>{entityType} details</SheetTitle></SheetHeader>
          <div className="p-4">
            {viewRow && <ViewEntity data={viewRow} schema={viewSchema} />}
          </div>
        </SheetContent>
      </Sheet>
    </div>
  );
}
