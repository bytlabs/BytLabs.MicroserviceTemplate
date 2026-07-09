'use client';
import Form, { FormProps } from '@rjsf/core';
import validator from '@rjsf/validator-ajv8';
import { theme } from './rjsf';

// Minimal, unopinionated wrapper around RJSF <Form>, themed with shadcn widgets. No layout chrome:
// the consumer decides where to place it (sheet, dialog, page, inline) and can override `className`
// and any RJSF Form prop. Pass children to replace RJSF's default submit button.
export interface DynamicFormProps extends Partial<FormProps> {
  schema: FormProps['schema'];
  onSubmit: FormProps['onSubmit'];
  className?: string;
}

export function DynamicForm({ className, children, ...props }: DynamicFormProps) {
  return (
    <Form
      validator={validator}
      templates={theme.templates}
      widgets={theme.widgets}
      className={className}
      {...props}
    >
      {children}
    </Form>
  );
}
