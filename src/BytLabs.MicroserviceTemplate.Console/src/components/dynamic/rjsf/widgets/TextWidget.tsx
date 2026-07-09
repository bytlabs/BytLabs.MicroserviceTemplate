import { WidgetProps } from "@rjsf/utils";
import { Input } from "@/components/ui/input";

export default function TextWidget(props: WidgetProps) {
  const { id, value, required, disabled, readonly, onChange, placeholder } = props;

  return (
    <Input
      id={id}
      value={value || ""}
      required={required}
      disabled={disabled || readonly}
      onChange={(event) => onChange(event.target.value)}
      placeholder={placeholder}
      className="w-full"
    />
  );
} 