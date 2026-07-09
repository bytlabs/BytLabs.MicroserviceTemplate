import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { ScrollArea } from '@/components/ui/scroll-area';
import Form from '@rjsf/core';
import { RJSFSchema, UiSchema } from '@rjsf/utils';
import validator from '@rjsf/validator-ajv8';
import { theme } from "@/components/dynamic/rjsf";
import { useState } from 'react';

interface FormPreviewProps<TFormData> {
  title: string;
  description: string;
  onSubmit: (data: TFormData) => Promise<void>;
  schema: RJSFSchema;
  uiSchema: UiSchema;
  className?: string;
  formData: any
}

export function CreateFormPreview<TFormData>({ 
  title, 
  description, 
  onSubmit, 
  schema, 
  uiSchema,
  className,
  formData
}: FormPreviewProps<TFormData>) {
  const [loading, setLoading] = useState(false);

  const handleSubmit = async ({ formData }: { formData?: TFormData }) => {
    setLoading(true);
    try {
      if (formData) {
        await onSubmit(formData);
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card className={className}>
      <CardHeader>
        <CardTitle>Form Preview</CardTitle>
        <CardDescription>You can test the form validations by submitting the form below.</CardDescription>
      </CardHeader>

      <CardContent>
        <ScrollArea className="h-full pr-4">
          <Form<any>
            id="preview-form"
            schema={schema}
            templates={theme.templates}
            widgets={theme.widgets}
            validator={validator}
            onSubmit={handleSubmit}
            className="space-y-4 pl-1"
            uiSchema={uiSchema}
            formData={formData}
          />
          <div className="flex justify-end space-x-2 mt-6">
            <Button type="submit" form="preview-form" disabled={loading}>
              {loading ? 'Submitting...' : 'Submit'}
            </Button>
          </div>
        </ScrollArea>
      </CardContent>
    </Card>
  );
} 