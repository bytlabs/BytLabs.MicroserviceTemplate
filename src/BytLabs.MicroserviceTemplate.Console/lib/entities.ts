import { gql, DocumentNode } from '@apollo/client';

// Registers a flat entity so the generic EntityManager can list/view/create/edit it. Adding a new
// entity (like Order) is adding one object here — no new UI code. The EntityDef (authored in the
// schema editor) supplies the form/table/view schema; this supplies the GraphQL wiring + mapping.
export interface EntityRegistration {
  type: string;
  listRoot: string;            // connection field name, e.g. 'products'
  knownColumns: string[];      // top-level scalar filter fields, e.g. ['name']
  listQuery: DocumentNode;
  createMutation: DocumentNode;
  updateMutation: DocumentNode;
  removeMutation: DocumentNode;
  toCreateInput: (formData: any) => any;
  toUpdateInput: (id: string, formData: any) => any;
  toRemoveInput: (id: string) => any;
  rowToFormData: (row: any) => any;
}

const newId = () => (typeof crypto !== 'undefined' && crypto.randomUUID ? crypto.randomUUID() : `${Date.now()}`);

export const PRODUCT: EntityRegistration = {
  type: 'Product',
  listRoot: 'products',
  knownColumns: ['name'],
  listQuery: gql`
    query GetProducts($first: Int, $after: String, $where: ProductFilterInput, $order: [ProductSortInput!]) {
      products(first: $first, after: $after, where: $where, order: $order) {
        totalCount
        pageInfo { hasNextPage hasPreviousPage startCursor endCursor }
        nodes { id name data }
      }
    }
  `,
  createMutation: gql`mutation CreateProduct($input: CreateProductInput!) { createProduct(input: $input) { product { id } errors { ... on ValidationError { message } ... on BusinessError { message } } } }`,
  updateMutation: gql`mutation UpdateProduct($input: UpdateProductInput!) { updateProduct(input: $input) { product { id } errors { ... on ValidationError { message } ... on BusinessError { message } } } }`,
  removeMutation: gql`mutation RemoveProduct($input: RemoveProductInput!) { removeProduct(input: $input) { id } }`,
  toCreateInput: (f) => ({ id: newId(), name: f?.name ?? 'Untitled', data: f ?? {} }),
  toUpdateInput: (id, f) => ({ id, name: f?.name ?? 'Untitled', data: f ?? {} }),
  toRemoveInput: (id) => ({ id }),
  rowToFormData: (row) => ({ name: row.name, ...(row.data ?? {}) }),
};

export const ENTITIES: Record<string, EntityRegistration> = {
  Product: PRODUCT,
};
