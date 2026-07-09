import {
  FormContextType,
  ObjectFieldTemplatePropertyType,
  ObjectFieldTemplateProps,
  RJSFSchema,
  StrictRJSFSchema,
  UIOptionsType,
  canExpand,
  descriptionId,
  getTemplate,
  getUiOptions,
  titleId,
} from '@rjsf/utils';

import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"

/** The `ObjectFieldTemplate` is the template to use to render all the inner properties of an object along with the
 * title and description if available. If the object is expandable, then an `AddButton` is also rendered after all
 * the properties.
 *
 * @param props - The `ObjectFieldTemplateProps` for this component
 */
export default function ObjectFieldTemplate<
  T = any,
  S extends StrictRJSFSchema = RJSFSchema,
  F extends FormContextType = any
>(props: ObjectFieldTemplateProps<T, S, F>) {
  const {
    description,
    disabled,
    formData,
    idSchema,
    onAddClick,
    properties,
    readonly,
    registry,
    required,
    schema,
    title,
    uiSchema,
  } = props;
  const options = getUiOptions<T, S, F>(uiSchema) as UIOptionsType<T,S,F> & { cardContentClassName: string, cardClassName: string, cardTitleClassName: string, cardHeaderClassName: string };  
  const TitleFieldTemplate = getTemplate<'TitleFieldTemplate', T, S, F>('TitleFieldTemplate', registry, options);
  const DescriptionFieldTemplate = getTemplate<'DescriptionFieldTemplate', T, S, F>(
    'DescriptionFieldTemplate',
    registry,
    options
  );
  // Button templates are not overridden in the uiSchema
  const {
    ButtonTemplates: { AddButton },
  } = registry.templates;

  return (
    <fieldset id={idSchema.$id}>

    <Card className={options.cardClassName}>
        {(title || description) && (<CardHeader className={options.cardHeaderClassName}>

        {title && (
          <CardTitle className={options.cardTitleClassName}>
            <TitleFieldTemplate
              id={titleId<T>(idSchema)}
              title={title}
              required={required}
              schema={schema}
              uiSchema={uiSchema}
              registry={registry}
            />
          </CardTitle>
        )}

        {description && (
          <CardDescription>
            <DescriptionFieldTemplate
              id={descriptionId<T>(idSchema)}
              description={description}
              schema={schema}
              uiSchema={uiSchema}
              registry={registry}
            />
          </CardDescription>
        )}


      </CardHeader>)}

      <CardContent className={options.cardContentClassName}>
        <div className="flex flex-col gap-6">
          {properties.map((prop: ObjectFieldTemplatePropertyType) => prop.content)}
        </div>

        {canExpand<T, S, F>(schema, uiSchema, formData) && (
          <AddButton
            className='object-property-expand'
            onClick={onAddClick(schema)}
            disabled={disabled || readonly}
            uiSchema={uiSchema}
            registry={registry}
          />
        )}
      </CardContent>
    </Card>

    </fieldset>
  );
}