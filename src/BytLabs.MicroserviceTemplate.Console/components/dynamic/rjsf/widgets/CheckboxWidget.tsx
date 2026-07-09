import { WidgetProps } from "@rjsf/utils";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";

export default function CheckboxWidget(props: WidgetProps) {
  const { id, value, required, disabled, readonly, onChange, label } = props;

  return (
    <div className="flex items-center space-x-2">
      <Checkbox
        id={id}
        checked={value || false}
        disabled={disabled || readonly}
        onCheckedChange={(checked) => onChange(checked)}
        required={required}
      />
      <Label htmlFor={id}>{label}</Label>
    </div>
  );
} 