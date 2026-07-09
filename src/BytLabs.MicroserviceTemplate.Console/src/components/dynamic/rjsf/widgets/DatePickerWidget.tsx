import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { format } from "date-fns";
import { CalendarIcon } from "lucide-react";
import { WidgetProps } from "@rjsf/utils";

// Parse the stored value into a Date for display. A plain "YYYY-MM-DD" is parsed as a *local*
// date (not UTC) so the calendar shows the day the user picked regardless of timezone.
const parseValue = (value: unknown): Date | undefined => {
  if (!value) return undefined;
  if (typeof value === "string" && /^\d{4}-\d{2}-\d{2}$/.test(value)) {
    const [y, m, d] = value.split("-").map(Number);
    return new Date(y, m - 1, d);
  }
  const parsed = new Date(value as string);
  return Number.isNaN(parsed.getTime()) ? undefined : parsed;
};

const DatePickerWidget = ({
  id,
  value,
  required,
  disabled,
  readonly,
  onChange,
  label,
  schema,
}: WidgetProps) => {
  const date = parseValue(value);
  // Emit a value matching the schema's declared format. `date` (JSON Schema full-date) requires
  // "YYYY-MM-DD"; a full ISO datetime fails AJV's `date` format check. `date-time` keeps the ISO string.
  const isDateTime = schema?.format === "date-time";

  return (
    <Popover modal={true}>
      <PopoverTrigger
        render={
          <Button
            variant="outline"
            className={cn(
              "w-full justify-start text-left font-normal",
              !date && "text-muted-foreground"
            )}
            disabled={disabled || readonly}
          />
        }
      >
        <CalendarIcon className="mr-2 h-4 w-4" />
        {date ? format(date, "PPP") : <span>Pick a date</span>}
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0" align="start">
        <Calendar
          mode="single"
          selected={date}
          onSelect={(date) => {
            if (!date) {
              onChange(undefined);
              return;
            }
            onChange(isDateTime ? date.toISOString() : format(date, "yyyy-MM-dd"));
          }}
          disabled={disabled || readonly}
          captionLayout="dropdown"
        />
      </PopoverContent>
    </Popover>
  );
};

export default DatePickerWidget;