import { createContext, useContext } from "react";

export type EntityOption = { value: string; label: string };

// Loads the selectable options for a referenced entity type (e.g. "Product").
export type EntityOptionsLoader = (entity: string) => Promise<EntityOption[]>;

const EntityOptionsContext = createContext<{ loadOptions: EntityOptionsLoader } | null>(null);

// Provide this around a <DynamicForm/> so ReferenceWidget fields can resolve their options.
// The host app supplies `loadOptions` (e.g. querying another entity's list via Apollo).
export const EntityOptionsProvider = EntityOptionsContext.Provider;

export function useEntityOptions() {
  return useContext(EntityOptionsContext);
}
