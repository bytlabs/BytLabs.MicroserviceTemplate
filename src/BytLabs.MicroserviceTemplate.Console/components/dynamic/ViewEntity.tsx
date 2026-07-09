'use client'

import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Label } from "@/components/ui/label";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { get } from "lodash";
import { cn } from "@/lib/utils";
import { format, parseISO, isValid } from "date-fns";
import Link from "next/link";

// Schema Types
type ComponentType =
  | "card" | "pipeline" | "table" | "dt" | "badge" | "tabs"
  | "timeline" | "notes" | "files" | "container" | "links";

type BindValue = string | Record<string, any>;

interface BaseComponent { type: ComponentType; bind?: BindValue; }
interface CardComponent extends BaseComponent { type: "card"; content: ViewComponent[]; title?: string; }
interface PipelineComponent extends BaseComponent {
  type: "pipeline";
  stages: Array<{ id: string; name: string; description?: string; status?: { bind: string } }>;
  bind: { currentStage: string };
  onStageChange?: (stageId: string) => void;
}
interface TableComponent extends BaseComponent {
  type: "table";
  bind: { rows: string; columns: Array<{ name: string; field: string }> };
}
interface DataTermComponent extends BaseComponent {
  type: "dt";
  label?: string;
  bind: string;
  format?: { type: "date"; format: string; fallback?: string }
    | { type: "number"; format: "currency" | "percentage" | "decimal"; options?: { currency?: string; minimumFractionDigits?: number; maximumFractionDigits?: number } };
}
interface TabsComponent extends BaseComponent { type: "tabs"; tabs: Array<{ id: string; label: string; content: ViewComponent[] }>; }
interface TimelineComponent extends BaseComponent { type: "timeline"; bind: { items: string; dateField: string; titleField: string; descriptionField: string }; }
interface NotesComponent extends BaseComponent { type: "notes"; bind: { notes: string }; }
interface FilesComponent extends BaseComponent { type: "files"; bind: { files: string }; }
interface ContainerComponent extends BaseComponent { type: "container"; content: ViewComponent[]; className?: string; }
interface LinksComponent extends BaseComponent {
  type: "links";
  links: Array<{ label: string; url: string; icon?: string; template?: boolean }>;
  bind?: { templateValues: string };
}

type ViewComponent =
  | CardComponent | PipelineComponent | TableComponent | DataTermComponent | TabsComponent
  | TimelineComponent | NotesComponent | FilesComponent | ContainerComponent | LinksComponent;

export interface ViewSchema {
  type: "container" | "card";
  content: ViewComponent[];
  title?: string;
}

interface ViewEntityProps<TData> {
  data: TData & { data?: Record<string, any> };
  schema: ViewSchema;
  // Optional tenant/org id woven into link queries.
  organizationId?: string;
  // Overridable wrapper class. No layout chrome (no Sheet) — wrap this component yourself.
  className?: string;
}

function resolveBind(data: any, bind: BindValue): any {
  if (typeof bind === 'string') return get(data, bind) ?? '-';
  return Object.entries(bind).reduce((acc, [key, path]) => {
    acc[key] = get(data, path);
    return acc;
  }, {} as Record<string, any>);
}

function formatValue(value: any, formatConfig?: DataTermComponent["format"]): string {
  if (!formatConfig) return String(value ?? '-');
  switch (formatConfig.type) {
    case "date":
      try {
        const date = typeof value === 'string' ? parseISO(value) : new Date(value);
        if (isValid(date)) return format(date, formatConfig.format);
        return formatConfig.fallback ?? '-';
      } catch {
        return formatConfig.fallback ?? '-';
      }
    case "number":
      const num = Number(value);
      if (isNaN(num)) return '-';
      switch (formatConfig.format) {
        case "currency":
          return new Intl.NumberFormat('en-US', { style: 'currency', currency: formatConfig.options?.currency ?? 'USD', minimumFractionDigits: formatConfig.options?.minimumFractionDigits ?? 2, maximumFractionDigits: formatConfig.options?.maximumFractionDigits ?? 2 }).format(num);
        case "percentage":
          return new Intl.NumberFormat('en-US', { style: 'percent', minimumFractionDigits: formatConfig.options?.minimumFractionDigits ?? 1, maximumFractionDigits: formatConfig.options?.maximumFractionDigits ?? 1 }).format(num / 100);
        case "decimal":
          return new Intl.NumberFormat('en-US', { minimumFractionDigits: formatConfig.options?.minimumFractionDigits ?? 0, maximumFractionDigits: formatConfig.options?.maximumFractionDigits ?? 2 }).format(num);
        default:
          return String(value ?? '-');
      }
  }
}

function replaceTemplateValues(url: string, templateValues: Record<string, any>) {
  return url.replace(/\{([^}]+)\}/g, (match, key) => (templateValues && key in templateValues ? templateValues[key] || match : ''));
}

interface RenderComponentOptions { organizationId?: string }

function renderComponent(component: ViewComponent, data: any, options: RenderComponentOptions, level = 0) {
  const resolvedBind = component.bind ? resolveBind(data, component.bind) : data;

  switch (component.type) {
    case "container":
      return (
        <div key={level} className={component.className}>
          {component.content.map((child, index) => <div key={index}>{renderComponent(child, data, options, level + 1)}</div>)}
        </div>
      );
    case "card":
      return (
        <Card key={level} className="shadow-sm border bg-muted/40 mb-4">
          <CardContent className="p-3">
            {component.title && <CardHeader className="p-0 mb-2"><h3 className="text-lg font-semibold">{component.title}</h3></CardHeader>}
            {component.content.map((child, index) => <div key={index}>{renderComponent(child, data, options, level + 1)}</div>)}
          </CardContent>
        </Card>
      );
    case "pipeline":
      const currentStage = resolvedBind?.currentStage;
      const totalStages = component.stages.length;
      return (
        <div className="space-y-4 mb-4 relative">
          <div className="grid gap-4" style={{ gridTemplateColumns: `repeat(${totalStages}, minmax(0, 1fr))`, columnGap: '1rem' }}>
            {component.stages.map((stage, index) => {
              const isCurrentStage = stage.id === currentStage;
              const isCompleted = component.stages.findIndex(s => s.id === currentStage) > index;
              return (
                <div key={stage.id} className="relative">
                  {index < component.stages.length - 1 && (
                    <div className={cn("absolute top-[15px] left-[calc(50%+16px)] right-0 h-[2px] transition-colors", isCompleted ? "bg-primary" : "bg-transparent")} style={{ width: 'calc(100% - 16px)' }} />
                  )}
                  <div className={cn("flex flex-col items-center cursor-pointer group relative", !isCompleted && !isCurrentStage && "cursor-not-allowed opacity-50")} onClick={() => component.onStageChange?.(stage.id)}>
                    <div className={cn("w-8 h-8 rounded-full flex items-center justify-center border-2 transition-colors bg-background relative z-10", isCompleted && "bg-primary border-primary text-primary-foreground", isCurrentStage && "border-primary bg-primary/10 text-primary", !isCompleted && !isCurrentStage && "border-muted text-muted-foreground")}>
                      <span className="text-sm font-medium">{index + 1}</span>
                    </div>
                    <div className="mt-3 text-center">
                      <div className="text-sm font-medium">{stage.name}</div>
                      {stage.description && <div className="text-xs text-muted-foreground mt-1">{stage.description}</div>}
                      {stage.status && <div className="text-xs text-muted-foreground mt-1">{formatValue(resolveBind(data, stage.status.bind))}</div>}
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      );
    case "table":
      const { rows, columns } = resolvedBind;
      return (
        <Table>
          <TableHeader>
            <TableRow>{columns.map((col: { name: string; field: string }, index: number) => <TableHead key={index}>{col.name}</TableHead>)}</TableRow>
          </TableHeader>
          <TableBody>
            {rows.map((row: any, rowIndex: number) => (
              <TableRow key={rowIndex}>{columns.map((col: { name: string; field: string }, colIndex: number) => <TableCell key={colIndex}>{row[col.field]}</TableCell>)}</TableRow>
            ))}
          </TableBody>
        </Table>
      );
    case "dt":
      return (
        <div className="grid grid-cols-3 items-start gap-4">
          <Label className="text-sm font-medium text-muted-foreground capitalize">{component.label || component.bind.split('.').pop()}</Label>
          <div className="col-span-2 text-sm">{formatValue(resolvedBind, component.format)}</div>
        </div>
      );
    case "tabs":
      return (
        <Tabs defaultValue={component.tabs[0].id}>
          <TabsList>{component.tabs.map((tab) => <TabsTrigger key={tab.id} value={tab.id}>{tab.label}</TabsTrigger>)}</TabsList>
          {component.tabs.map((tab) => (
            <TabsContent key={tab.id} value={tab.id}>
              {tab.content.map((child, index) => <div key={index}>{renderComponent(child, data, options, level + 1)}</div>)}
            </TabsContent>
          ))}
        </Tabs>
      );
    case "timeline":
      const { items, dateField, titleField, descriptionField } = resolvedBind;
      return (
        <div className="space-y-4">
          {items.map((item: any, index: number) => (
            <div key={index} className="relative pl-8 pb-4">
              <div className="absolute left-0 top-0 h-full w-px bg-border" />
              <div className="absolute left-0 top-0 h-4 w-4 rounded-full bg-primary" />
              <div>
                <div className="text-sm font-medium">{item[titleField]}</div>
                <div className="text-sm text-muted-foreground">{item[dateField]}</div>
                <div className="text-sm mt-1">{item[descriptionField]}</div>
              </div>
            </div>
          ))}
        </div>
      );
    case "notes":
      return (
        <div className="space-y-2">
          {resolvedBind?.notes.map((note: any, index: number) => (
            <Card key={index} className="p-3"><div className="text-sm">{note.content}</div><div className="text-xs text-muted-foreground mt-1">{note.createdAt}</div></Card>
          ))}
        </div>
      );
    case "files":
      return (
        <div className="space-y-2">
          {resolvedBind?.files.map((file: any, index: number) => (
            <Card key={index} className="p-3">
              <div className="flex items-center justify-between">
                <div><div className="text-sm font-medium">{file.name}</div><div className="text-xs text-muted-foreground">{file.size}</div></div>
                <Badge variant="secondary">{file.type}</Badge>
              </div>
            </Card>
          ))}
        </div>
      );
    case "links":
      return (
        <div className="space-y-2 mb-2">
          {component.links.map((link) => {
            const finalUrl = link.template ? replaceTemplateValues(link.url, resolvedBind) : link.url;
            return (
              <Link key={link.url} href={{ pathname: finalUrl, query: options.organizationId ? { teamId: options.organizationId } : undefined }} className="block hover:bg-muted rounded-lg transition-colors">
                <Card className="p-3">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-2">{link.icon}<div><div className="text-sm font-medium text-primary">{link.label}</div></div></div>
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="text-muted-foreground"><path d="M5 12h14" /><path d="m12 5 7 7-7 7" /></svg>
                  </div>
                </Card>
              </Link>
            );
          })}
        </div>
      );
    default:
      return null;
  }
}

// Unopinionated: renders the resolved view content only. Wrap in your own Sheet/Dialog/page as needed.
export function ViewEntity<TData>({ data, schema, organizationId, className }: ViewEntityProps<TData>) {
  return <div className={className}>{renderComponent(schema, data, { organizationId })}</div>;
}
