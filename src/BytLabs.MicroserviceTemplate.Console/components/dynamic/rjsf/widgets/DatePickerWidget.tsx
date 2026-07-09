import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { format } from "date-fns";
import { CalendarIcon } from "lucide-react";
import { WidgetProps } from "@rjsf/utils";

const DatePickerWidget = ({
  id,
  value,
  required,
  disabled,
  readonly,
  onChange,
  label,
}: WidgetProps) => {
  const date = value ? new Date(value) : undefined;

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
            onChange(date?.toISOString());
          }}
          disabled={disabled || readonly}
          captionLayout="dropdown"
        />
      </PopoverContent>
    </Popover>
  );
};

export default DatePickerWidget;