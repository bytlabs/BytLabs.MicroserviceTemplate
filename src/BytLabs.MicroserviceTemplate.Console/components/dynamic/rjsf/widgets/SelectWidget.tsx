import { WidgetProps } from "@rjsf/utils";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

export default function SelectWidget(props: WidgetProps) {
  const { id, options, value, required, disabled, readonly, onChange } = props;
  const { enumOptions } = options;

  return (
    <Select
      value={value || "None"}
      onValueChange={(v) => onChange(v)}
      disabled={disabled || readonly}
    >
      <SelectTrigger id={id} className="w-full">
        <SelectValue placeholder="Select an option" />
      </SelectTrigger>
      <SelectContent>
        {!required && (
          <SelectItem value="None">
            <em>None</em>
          </SelectItem>
        )}
        {enumOptions?.map((option: any) => (
          <SelectItem key={option.value} value={option.value}>
            {option.label}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
} 