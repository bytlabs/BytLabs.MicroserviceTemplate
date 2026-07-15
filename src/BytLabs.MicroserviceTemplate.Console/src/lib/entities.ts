import { gql, DocumentNode } from '@apollo/client';

// Registers a flat entity so the generic EntityManager can list/view/create/edit it. The EntityDef
// (authored in the schema editor) supplies the form/table/view schema; this supplies the GraphQL
// wiring. Inputs are built generically from the form data (see lib/inputs.ts) — no per-entity mappers,
// because the form nests dynamic fields under `data` and keeps outer fields at the top level, so
// `formData` already matches the mutation input shape.
export interface EntityRegistration {
  type: string;
  listRoot: string;            // connection field name, e.g. 'products'
  knownColumns: string[];      // top-level scalar filter fields, e.g. ['name']
  idField: string;             // id property name on the create input ('id' | 'orderId')
  labelField: string;          // field shown when this entity is picked from another (ReferenceWidget)
  rowIdFields?: string[];      // collection fields whose GraphQL input requires a row id (Order.items)
  listQuery: DocumentNode;
  createMutation: DocumentNode;
  updateMutation: DocumentNode;
  removeMutation: DocumentNode;
}

export const PRODUCT: EntityRegistration = {
  type: 'Product',
  listRoot: 'products',
  knownColumns: ['name'],
  idField: 'id',
  labelField: 'name',
  listQuery: gql`
    query GetProducts($first: Int, $after: String, $where: ProductFilterInput, $order: [ProductSortInput!]) {
      products(first: $first, after: $after, where: $where, order: $order) {
        totalCount
        pageInfo { hasNextPage hasPreviousPage startCursor endCursor }
        nodes { id name data variants { sku price } }
      }
    }
  `,
  createMutation: gql`mutation CreateProduct($input: CreateProductInput!) { createProduct(input: $input) { product { id } errors { ... on ValidationError { message } ... on BusinessError { message } } } }`,
  updateMutation: gql`mutation UpdateProduct($input: UpdateProductInput!) { updateProduct(input: $input) { product { id } errors { ... on ValidationError { message } ... on BusinessError { message } } } }`,
  removeMutation: gql`mutation RemoveProduct($input: RemoveProductInput!) { removeProduct(input: $input) { id } }`,
};

export const ORDER: EntityRegistration = {
  type: 'Order',
  listRoot: 'orders',
  knownColumns: [],
  idField: 'orderId',
  labelField: 'id',
  rowIdFields: ['items'], // OrderItemInput requires an id per row (the domain regenerates it anyway)
  listQuery: gql`
    query GetOrders($first: Int, $after: String, $where: OrderFilterInput, $order: [OrderSortInput!]) {
      orders(first: $first, after: $after, where: $where, order: $order) {
        totalCount
        pageInfo { hasNextPage hasPreviousPage startCursor endCursor }
        nodes { id orderDate status data items { productId quantity price } }
      }
    }
  `,
  createMutation: gql`mutation CreateOrder($input: CreateOrderInput!) { createOrder(input: $input) { createOrderResult { orderId } errors { ... on ValidationError { message } ... on BusinessError { message } } } }`,
  updateMutation: gql`mutation UpdateOrder($input: UpdateOrderInput!) { updateOrder(input: $input) { order { id } errors { ... on ValidationError { message } ... on BusinessError { message } } } }`,
  removeMutation: gql`mutation RemoveOrder($input: RemoveOrderInput!) { removeOrder(input: $input) { order { id } errors { ... on ValidationError { message } ... on BusinessError { message } } } }`,
};

export const ENTITIES: Record<string, EntityRegistration> = {
  Product: PRODUCT,
  Order: ORDER,
};
