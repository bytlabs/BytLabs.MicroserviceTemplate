import { Label } from '@/components/ui/label';
import {
  FieldTemplateProps,
  FormContextType,
  RJSFSchema,
  StrictRJSFSchema,
  getTemplate,
  getUiOptions,
} from '@rjsf/utils';


/** The `FieldTemplate` component is the template used by `SchemaField` to render any field. It renders the field
 * content, (label, description, children, errors and help) inside of a `WrapIfAdditional` component.
 *
 * @param props - The `FieldTemplateProps` for this component
 */
export default function FieldTemplate<
  T = any,
  S extends StrictRJSFSchema = RJSFSchema,
  F extends FormContextType = any
>(props: FieldTemplateProps<T, S, F>) {
  const { id, label, children, errors, help, description, hidden, required, displayLabel, registry, uiSchema } = props;
  const uiOptions = getUiOptions(uiSchema);
  const WrapIfAdditionalTemplate = getTemplate<'WrapIfAdditionalTemplate', T, S, F>(
    'WrapIfAdditionalTemplate',
    registry,
    uiOptions
  );
  if (hidden) {
    return <div className='hidden'>{children}</div>;
  }
  return (
    <WrapIfAdditionalTemplate {...props}>      
        <div className="grid gap-2">
          {displayLabel && <Label children={label} id={id} htmlFor={id} />}
          {displayLabel && description ? description : null}
          {children}
          {errors}
          {help}
        </div>
    </WrapIfAdditionalTemplate>
  );
}
