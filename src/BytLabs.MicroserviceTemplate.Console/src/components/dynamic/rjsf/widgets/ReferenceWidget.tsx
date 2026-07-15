import { useEffect, useState } from "react";
import { WidgetProps } from "@rjsf/utils";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { useEntityOptions, EntityOption } from "../EntityOptionsContext";

// Picks a record of another entity (an association). Configure via uiSchema:
//   "ui:widget": "ReferenceWidget",
//   "ui:options": { "entity": "Product", "labelField": "name" }
// Options come from the surrounding <EntityOptionsProvider/>; the stored value is the referenced id.
// With no provider (or no `entity` option) it degrades to a plain text input so the widget is
// usable standalone.
export default function ReferenceWidget(props: WidgetProps) {
  const { id, value, required, disabled, readonly, onChange, options } = props;
  const ctx = useEntityOptions();
  const entity = (options?.entity as string | undefined) ?? undefined;

  const [items, setItems] = useState<EntityOption[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!ctx || !entity) return;
    let active = true;
    setLoading(true);
    ctx
      .loadOptions(entity)
      .then((opts) => {
        if (active) setItems(opts);
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => {
      active = false;
    };
  }, [ctx, entity]);

  // Fallback: no provider or no configured entity -> plain text input.
  if (!ctx || !entity) {
    return (
      <Input
        id={id}
        value={value ?? ""}
        required={required}
        disabled={disabled || readonly}
        onChange={(e) => onChange(e.target.value || undefined)}
      />
    );
  }

  return (
    <Select
      value={value || ""}
      onValueChange={(v) => onChange(v || undefined)}
      disabled={disabled || readonly || loading}
    >
      <SelectTrigger id={id} className="w-full">
        <SelectValue placeholder={loading ? "Loading…" : `Select ${entity}`} />
      </SelectTrigger>
      <SelectContent>
        {items.map((opt) => (
          <SelectItem key={opt.value} value={opt.value}>
            {opt.label}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}
