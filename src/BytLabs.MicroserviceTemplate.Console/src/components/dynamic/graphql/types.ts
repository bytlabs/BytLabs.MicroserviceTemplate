export interface DataSchema { type: string; data: string }
export interface FormDataSchema { key: string; formSchema: DataSchema; formUi: DataSchema; sampleData: DataSchema }
export interface TableDataSchema { columns: DataSchema; filter: DataSchema; sampleData: DataSchema; details: DataSchema }
export interface EntityDef { id: string; entityType: string; form: FormDataSchema; table: TableDataSchema }

export type InputMaybe<T> = T | null | undefined;

export enum SortOrder { Asc = 'ASC', Desc = 'DESC' }
export enum SortEnumType { Asc = 'ASC', Desc = 'DESC' }
export enum FilterOperation { Contains = 'CONTAINS', Eq = 'EQ', Neq = 'NEQ', Gt = 'GT', Gte = 'GTE', Lt = 'LT', Lte = 'LTE' }
export enum ValueKind { String = 'String', Number = 'Number', Boolean = 'Boolean', Date = 'Date' }

export interface DataOperationFilterInput {
  path: string;
  operation: FilterOperation;
  value: string;
  valueType: ValueKind;
}
