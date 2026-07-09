'use client';
import { useEffect, useState } from 'react';
import { SchemaEditor } from '@/components/dynamic/SchemaEditor';
import { useEntityDef, EntityDefFormData } from '@/components/dynamic/graphql/useEntityDef';

function parse<T>(raw: string | undefined, fallback: T): T {
  if (!raw) return fallback;
  try { return JSON.parse(raw); } catch { return fallback; }
}

export function SchemaAuthoring({ entityType }: { entityType: string }) {
  const { def, loading, handleCreate, handleUpdate, handleDelete } = useEntityDef(entityType);

  const [formSchema, setFormSchema] = useState<any>({ type: 'object', properties: {} });
  const [uiSchema, setUiSchema] = useState<any>({});
  const [formData, setFormData] = useState<any>({});
  const [columns, setColumns] = useState<any[]>([]);
  const [details, setDetails] = useState<any>({ type: 'container', content: [] });
  const [tableData, setTableData] = useState<any[]>([]);
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);

  // Hydrate editors from the current def once it loads.
  useEffect(() => {
    if (!def) return;
    setFormSchema(parse(def.form?.formSchema?.data, { type: 'object', properties: {} }));
    setUiSchema(parse(def.form?.formUi?.data, {}));
    setFormData(parse(def.form?.sampleData?.data, {}));
    setColumns(parse(def.table?.columns?.data, []));
    setDetails(parse(def.table?.details?.data, { type: 'container', content: [] }));
    setTableData(parse(def.table?.sampleData?.data, []));
  }, [def]);

  const onSave = async () => {
    setSaving(true);
    try {
      const payload: EntityDefFormData = {
        form: { formSchema, uiSchema, sampleData: formData },
        table: { columns, filter: {}, sampleData: tableData, details },
      };
      if (def) await handleUpdate(def.id, payload);
      else await handleCreate(payload);
    } finally {
      setSaving(false);
    }
  };

  const onDelete = def
    ? async () => { setDeleting(true); try { await handleDelete(def.id); } finally { setDeleting(false); } }
    : undefined;

  if (loading) return <div className="p-8">Loading…</div>;

  return (
    <div className="container mx-auto py-8">
      <SchemaEditor
        title={`${entityType} schema`}
        description="Author the form, table, and detail-view schema for this entity."
        formSchema={formSchema}
        uiSchema={uiSchema}
        formData={formData}
        columns={columns}
        details={details}
        tableData={tableData}
        onFormSchemaChange={setFormSchema}
        onUiSchemaChange={setUiSchema}
        onFormDataChange={setFormData}
        onColumnsChange={setColumns}
        onDetailsChange={setDetails}
        onTableDataChange={setTableData}
        onSave={onSave}
        onDelete={onDelete}
        loading={saving}
        deleteLoading={deleting}
      />
    </div>
  );
}
