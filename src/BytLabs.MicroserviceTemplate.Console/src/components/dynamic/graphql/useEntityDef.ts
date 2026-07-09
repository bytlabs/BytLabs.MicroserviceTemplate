'use client';
import { gql, useMutation, useQuery } from '@apollo/client';
import { v4 as uuid } from 'uuid';
import type { EntityDef } from './types';

const ENTITY_DEFS = gql`
  query EntityDefs($first: Int, $where: EntityDefFilterInput) {
    entityDefs(first: $first, where: $where) {
      nodes {
        id entityType
        form { key formSchema { type data } formUi { type data } sampleData { type data } }
        table { columns { type data } filter { type data } sampleData { type data } details { type data } }
      }
    }
  }
`;
const CREATE = gql`mutation CreateEntityDef($input: CreateEntityDefInput!) { createEntityDef(input: $input) { entityDef { id } errors { ... on ValidationError { message } ... on BusinessError { message } } } }`;
const UPDATE = gql`mutation UpdateEntityDef($input: UpdateEntityDefInput!) { updateEntityDef(input: $input) { entityDef { id } errors { ... on ValidationError { message } ... on BusinessError { message } } } }`;
const REMOVE = gql`mutation RemoveEntityDef($input: RemoveEntityDefInput!) { removeEntityDef(input: $input) { entityDef { id } errors { ... on ValidationError { message } ... on BusinessError { message } } } }`;

export type EntityDefFormData = {
  form: { formSchema: any; uiSchema: any; sampleData: any };
  table: { columns: any; filter: any; sampleData: any; details: any };
};

const toFormInput = (entityType: string, d: EntityDefFormData) => ({
  key: entityType,
  formSchema: { type: 'rjsf/formSchema', data: JSON.stringify(d.form.formSchema) },
  formUi: { type: 'rjsf/uiSchema', data: JSON.stringify(d.form.uiSchema) },
  sampleData: { type: 'json', data: JSON.stringify(d.form.sampleData) },
});
const toTableInput = (d: EntityDefFormData) => ({
  columns: { type: 'tanstack/columnDef', data: JSON.stringify(d.table.columns) },
  filter: { type: 'json', data: JSON.stringify(d.table.filter) },
  sampleData: { type: 'json', data: JSON.stringify(d.table.sampleData) },
  details: { type: 'cms/view', data: JSON.stringify(d.table.details) },
});

// Reads and authors the EntityDef for a given entity type. Wire an ApolloProvider above this hook.
export function useEntityDef(entityType: string) {
  const { data, loading, error, refetch } = useQuery(ENTITY_DEFS, {
    variables: { where: { entityType: { eq: entityType } }, first: 1 },
  });
  const def = data?.entityDefs?.nodes?.[0] as EntityDef | undefined;

  const [createDef] = useMutation(CREATE);
  const [updateDef] = useMutation(UPDATE);
  const [removeDef] = useMutation(REMOVE);

  const handleCreate = async (d: EntityDefFormData) => {
    const res = await createDef({ variables: { input: { id: uuid(), entityType, form: toFormInput(entityType, d), table: toTableInput(d) } } });
    if (res.data?.createEntityDef.errors?.length) throw res.data.createEntityDef.errors[0];
    await refetch();
    return res.data?.createEntityDef.entityDef;
  };
  const handleUpdate = async (id: string, d: EntityDefFormData) => {
    const res = await updateDef({ variables: { input: { id, form: toFormInput(entityType, d), table: toTableInput(d) } } });
    if (res.data?.updateEntityDef.errors?.length) throw res.data.updateEntityDef.errors[0];
    await refetch();
    return res.data?.updateEntityDef.entityDef;
  };
  const handleDelete = async (id: string) => {
    const res = await removeDef({ variables: { input: { id } } });
    if (res.data?.removeEntityDef.errors?.length) throw res.data.removeEntityDef.errors[0];
    await refetch();
  };

  return { def, loading, error, refetch, handleCreate, handleUpdate, handleDelete };
}
