import { useMemo, useState, useRef } from "react";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Button } from "@/components/ui/button";
import { Editor } from "@monaco-editor/react";
import { RJSFSchema, UiSchema } from "@rjsf/utils";
import { DynamicColumnDef } from "@/components/dynamic/CreateColumns";
import { ViewSchema } from "@/components/dynamic/ViewEntity";
import { CreateFormPreview } from "@/components/dynamic/CreateFormPreview";
import { DataTablePreview } from "@/components/dynamic/DataTablePreview";

export interface SchemaEditorProps {
  title: string;
  description: string;
  formSchema: RJSFSchema;
  uiSchema: UiSchema;
  formData: any;
  columns: DynamicColumnDef[];
  details: ViewSchema;
  tableData: any[];
  onFormSchemaChange: (schema: RJSFSchema) => void;
  onUiSchemaChange: (schema: UiSchema) => void;
  onFormDataChange: (data: any) => void;
  onColumnsChange: (columns: DynamicColumnDef[]) => void;
  onDetailsChange: (details: ViewSchema) => void;
  onTableDataChange: (data: any[]) => void;
  onSave: () => Promise<void>;
  onDelete?: () => Promise<void>;
  loading?: boolean;
  deleteLoading?: boolean;
}

export function SchemaEditor({
  title,
  description,
  formSchema,
  uiSchema,
  formData,
  columns,
  details,
  tableData,
  onFormSchemaChange,
  onUiSchemaChange,
  onFormDataChange,
  onColumnsChange,
  onDetailsChange,
  onTableDataChange,
  onSave,
  onDelete,
  loading,
  deleteLoading
}: SchemaEditorProps) {
  const [activeTab, setActiveTab] = useState("form");
  const fileInputRef = useRef<HTMLInputElement>(null);

  const formSchemaEditorVal = useMemo(()=> JSON.stringify(formSchema, null, 2), [formSchema]);
  const uiSchemaEditorVal = useMemo(()=> JSON.stringify(uiSchema, null, 2), [uiSchema]);
  const formDataEditorVal = useMemo(()=>JSON.stringify(formData, null, 2), [formData]);
  const columnsEditorVal = useMemo(()=> JSON.stringify(columns, null, 2), [columns]);
  const detailsEditorVal = useMemo(()=> JSON.stringify(details, null, 2), [details]);
  const tableDataEditorVal = useMemo(()=> JSON.stringify(tableData, null, 2), [tableData]);

  const handleExportConfig = () => {
    const config = {
      formSchema,
      uiSchema,
      formData,
      columns,
      details,
      tableData
    };
    
    const blob = new Blob([JSON.stringify(config, null, 2)], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'schema-config.json';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  };

  const handleImportConfig = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = (e) => {
      try {
        const config = JSON.parse(e.target?.result as string);
        if (config.formSchema) onFormSchemaChange(config.formSchema);
        if (config.uiSchema) onUiSchemaChange(config.uiSchema);
        if (config.formData) onFormDataChange(config.formData);
        if (config.columns) onColumnsChange(config.columns);
        if (config.details) onDetailsChange(config.details);
        if (config.tableData) onTableDataChange(config.tableData);
      } catch {
        alert('Invalid configuration file');
      }
    };
    reader.readAsText(file);
     event.target.value = ''; // Clear the input value to allow re-uploading the same file
  };

  return (
    <div className="h-full flex-1 flex flex-col space-y-8">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold tracking-tight">{title}</h2>
          <p className="text-muted-foreground">{description}</p>
        </div>
        <div className="flex items-center space-x-2">
          <input
            type="file"
            ref={fileInputRef}
            onChange={handleImportConfig}
            accept=".json"
            className="hidden"
          />
          <Button
            variant="outline"
            onClick={() => fileInputRef.current?.click()}
          >
            Import
          </Button>
          <Button
            variant="outline"
            onClick={handleExportConfig}
          >
            Export
          </Button>
          {onDelete && (
            <Button variant="destructive" onClick={onDelete} disabled={deleteLoading}>
              {deleteLoading ? "Deleting..." : "Delete"}
            </Button>
          )}
          <Button onClick={onSave} disabled={loading}>
            {loading ? "Saving..." : "Save Configuration"}
          </Button>
        </div>
      </div>

      <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
        <div className="flex justify-center">
          <TabsList className="grid w-[400px] grid-cols-2 mb-4">
            <TabsTrigger value="form">Form</TabsTrigger>
            <TabsTrigger value="table">Table</TabsTrigger>
          </TabsList>
        </div>

        <TabsContent value="form" className="flex-1 mt-0">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 min-h-[800px] rounded-lg border p-2">
            <div>
              <Tabs defaultValue="schema" className="flex flex-col h-full">
                <TabsList className="w-full justify-start">
                  <TabsTrigger value="schema">Form Schema</TabsTrigger>
                  <TabsTrigger value="uiSchema">UI Schema</TabsTrigger>
                  <TabsTrigger value="formData">Sample Data</TabsTrigger>
                </TabsList>
                <TabsContent value="schema" className="flex-1">
                  <Editor
                    height="100%"
                    defaultLanguage="json"
                    value={formSchemaEditorVal}
                    onChange={(value) => {
                      try {
                        if (value) {
                          onFormSchemaChange(JSON.parse(value));
                        }
                      } catch (e) {
                        // Invalid JSON, ignore
                      }
                    }}
                    options={{
                      minimap: { enabled: false },
                      fontSize: 14,
                    }}
                  />
                </TabsContent>
                <TabsContent value="uiSchema" className="flex-1">
                  <Editor
                    height="100%"
                    defaultLanguage="json"
                    value={uiSchemaEditorVal}
                    onChange={(value) => {
                      try {
                        if (value) {
                          onUiSchemaChange(JSON.parse(value));
                        }
                      } catch (e) {
                        // Invalid JSON, ignore
                      }
                    }}
                    options={{
                      minimap: { enabled: false },
                      fontSize: 14,
                    }}
                  />
                </TabsContent>
                <TabsContent value="formData" className="flex-1">
                  <Editor
                    height="100%"
                    defaultLanguage="json"
                    value={formDataEditorVal}
                    onChange={(value) => {
                      try {
                        if (value) {
                          onFormDataChange(JSON.parse(value));
                        }
                      } catch (e) {
                        // Invalid JSON, ignore
                      }
                    }}
                    options={{
                      minimap: { enabled: false },
                      fontSize: 14,
                    }}
                  />
                </TabsContent>
              </Tabs>
            </div>
            <div>
              <div className="h-full p-6">
                <CreateFormPreview
                  title="Form Preview"
                  description="Preview of how the form will appear"
                  onSubmit={async () => { /* preview only */ }}
                  schema={formSchema}
                  uiSchema={uiSchema}
                  formData={formData}
                  className="h-full"
                />
              </div>
            </div>
          </div>
        </TabsContent>

        <TabsContent value="table" className="flex-1 mt-0">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 min-h-[800px] rounded-lg border p-2">
            <div>
              <Tabs defaultValue="columns" className="flex flex-col h-full">
                <TabsList className="w-full justify-start">
                  <TabsTrigger value="columns">Column Schema</TabsTrigger>
                  <TabsTrigger value="details">View Schema</TabsTrigger>
                  <TabsTrigger value="tableData">Sample Data</TabsTrigger>
                </TabsList>
                <TabsContent value="columns" className="flex-1">
                  <Editor
                    height="100%"
                    defaultLanguage="json"
                    value={columnsEditorVal}
                    onChange={(value) => {
                      try {
                        if (value) {
                          onColumnsChange(JSON.parse(value));
                        }
                      } catch (e) {
                        // Invalid JSON, ignore
                      }
                    }}
                    options={{
                      minimap: { enabled: false },
                      fontSize: 14,
                    }}
                  />
                </TabsContent>
                <TabsContent value="details" className="flex-1">
                  <Editor
                    height="100%"
                    defaultLanguage="json"
                    value={detailsEditorVal}
                    onChange={(value) => {
                      try {
                        if (value) {
                          onDetailsChange(JSON.parse(value));
                        }
                      } catch (e) {
                        // Invalid JSON, ignore
                      }
                    }}
                    options={{
                      minimap: { enabled: false },
                      fontSize: 14,
                    }}
                  />
                </TabsContent>
                <TabsContent value="tableData" className="flex-1">
                  <Editor
                    height="100%"
                    defaultLanguage="json"
                    value={tableDataEditorVal}
                    onChange={(value) => {
                      try {
                        if (value) {
                          onTableDataChange(JSON.parse(value));
                        }
                      } catch (e) {
                        // Invalid JSON, ignore
                      }
                    }}
                    options={{
                      minimap: { enabled: false },
                      fontSize: 14,
                    }}
                  />
                </TabsContent>
              </Tabs>
            </div>
            <div>
              <div className="h-full p-6">
                <DataTablePreview
                  columns={columns}
                  sampleData={tableData}
                  details={details}
                />
              </div>
            </div>
          </div>
        </TabsContent>
      </Tabs>
    </div>
  );
}