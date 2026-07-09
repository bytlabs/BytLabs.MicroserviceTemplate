import { WidgetProps } from "@rjsf/utils";
import { Textarea } from "@/components/ui/textarea";

export default function TextareaWidget(props: WidgetProps) {
  const { id, value, required, disabled, readonly, onChange, placeholder } = props;

  return (
    <Textarea
      id={id}
      value={value || ""}
      required={required}
      disabled={disabled || readonly}
      onChange={(event) => onChange(event.target.value)}
      placeholder={placeholder}
      className="w-full min-h-[100px]"
    />
  );
} 