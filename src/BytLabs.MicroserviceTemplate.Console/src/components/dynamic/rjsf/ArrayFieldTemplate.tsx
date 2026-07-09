import { ArrayFieldTemplateProps } from "@rjsf/utils";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Plus, Trash2 } from "lucide-react";

export default function ArrayFieldTemplate(props: ArrayFieldTemplateProps) {
  const { items, canAdd, onAddClick, title, required } = props;

  return (
    <Card className="w-full">
      {title && (
        <CardHeader className="text-lg font-semibold">
          {title}
          {required && <span className="text-red-500 ml-1">*</span>}
        </CardHeader>
      )}
      <CardContent className="space-y-4">
        {items.map(({ key, children, onDropIndexClick, hasRemove, index }) => (
          <div key={key} className="flex items-center gap-2">
            <div className="flex-grow">{children}</div>
            {hasRemove && (
              <Button
                variant="destructive"
                size="icon"
                onClick={onDropIndexClick(index)}
                className="shrink-0"
              >
                <Trash2 className="h-4 w-4" />
              </Button>
            )}
          </div>
        ))}
        {canAdd && (
          <Button
            type="button"
            onClick={onAddClick}
            variant="outline"
            className="w-full"
          >
            <Plus className="h-4 w-4 mr-2" />
            Add Item
          </Button>
        )}
      </CardContent>
    </Card>
  );
} 