import { ThemeProps } from "@rjsf/core";
import { RJSFSchema, UiSchema } from "@rjsf/utils";
import ArrayFieldTemplate from "./ArrayFieldTemplate";
import FieldTemplate from "./FieldTemplate";
import ObjectFieldTemplate from "./ObjectFieldTemplate";
import CheckboxWidget from "./widgets/CheckboxWidget";
import SelectWidget from "./widgets/SelectWidget";
import TextareaWidget from "./widgets/TextareaWidget";
import TextWidget from "./widgets/TextWidget";
import DatePickerWidget from "./widgets/DatePickerWidget";
import UpDownWidget from "./widgets/UpDownWidget";

// Base theme mapping JSON-Schema fields to shadcn inputs. To add domain-specific widgets, spread
// `theme.widgets` into your own theme and pass it to <Form/>.
export const theme: ThemeProps<any, RJSFSchema, UiSchema> = {
  templates: { ArrayFieldTemplate, FieldTemplate, ObjectFieldTemplate },
  widgets: { TextWidget, TextareaWidget, SelectWidget, CheckboxWidget, DatePickerWidget, UpDownWidget },
};
