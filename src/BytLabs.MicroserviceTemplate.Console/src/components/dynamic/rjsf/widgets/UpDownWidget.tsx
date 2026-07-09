import { WidgetProps } from "@rjsf/utils";
import { Input } from "@/components/ui/input";

export default function UpDownWidget(props: WidgetProps) {
  const { id, value, required, disabled, readonly, onChange, schema } = props;
  const step = schema.multipleOf || 1;

  return (
    <Input
    id={id}
    type="number"
    value={value || ""}
    required={required}
    disabled={disabled || readonly}
    onChange={(event) => {
      const val = event.target.value;
      onChange(val ? Number(val) : undefined);
    }}
    step={step}
    className="w-full"
  />
  );
}