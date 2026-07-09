import * as React from "react"
import { Column } from "@tanstack/react-table"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuTrigger,
  DropdownMenuItem,
} from "@/components/ui/dropdown-menu"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Filter, FilterX } from "lucide-react"
import { FilterOperation } from "@/components/dynamic/graphql/types"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { ScrollArea } from "@/components/ui/scroll-area"

export interface FilterCondition {
  value: string
  operation: FilterOperation
}

export interface FilterState {
  conditions: FilterCondition[]
}

const filterOperationLabels: Record<FilterOperation, string> = {
  [FilterOperation.Eq]: "Equals",
  [FilterOperation.Neq]: "Not Equal",
  [FilterOperation.Contains]: "Contains",
  [FilterOperation.Gt]: "Greater Than",
  [FilterOperation.Gte]: "Greater Than or Equal",
  [FilterOperation.Lt]: "Less Than",
  [FilterOperation.Lte]: "Less Than or Equal",
}

interface FilterPopoverProps {
  column: Column<any, any>
  activeFilter: FilterState | null
  onFilterChange: (conditions: FilterCondition[]) => void
}

// Generic column filter: one or more (operation, value) conditions. Domain-specific value pickers
// (e.g. an async entity select) can be layered on by wrapping this component.
const FilterPopover = React.memo(({ column, activeFilter, onFilterChange }: FilterPopoverProps) => {
  const [isOpen, setIsOpen] = React.useState(false)
  const [localConditions, setLocalConditions] = React.useState<FilterCondition[]>(
    activeFilter?.conditions || []
  )

  const addCondition = () => {
    setLocalConditions([...localConditions, { value: "", operation: FilterOperation.Contains }])
  }

  const updateCondition = (index: number, value: string, operation: FilterOperation) => {
    const newConditions = [...localConditions]
    newConditions[index] = { value, operation }
    setLocalConditions(newConditions)
  }

  const removeCondition = (index: number) => {
    setLocalConditions(localConditions.filter((_, i) => i !== index))
  }

  const handleApply = () => {
    onFilterChange(localConditions.filter((c) => c.value.trim() !== ""))
    setIsOpen(false)
  }

  const handleClear = () => {
    setLocalConditions([])
    onFilterChange([])
    setIsOpen(false)
  }

  return (
    <Popover open={isOpen} onOpenChange={setIsOpen}>
      <PopoverTrigger render={<Button variant="ghost" size="sm" className="h-8 w-8 p-0" />}>{activeFilter?.conditions.length ? (
                      <Badge variant="secondary" className="rounded-sm px-1 font-normal">
                        {activeFilter.conditions.length}
                      </Badge>
                    ) : (
                      <Filter className="h-4 w-4" />
                    )}</PopoverTrigger>
      <PopoverContent className="w-80">
        <div className="space-y-4">
          <div className="flex items-center justify-between">
            <Label>Filter {column.columnDef.header?.toString()}</Label>
            <Button variant="ghost" size="sm" onClick={addCondition}>Add Filter</Button>
          </div>
          <ScrollArea className="h-[300px] pr-4">
            <div className="space-y-4">
              {localConditions.map((condition, index) => (
                <div key={index} className="space-y-2 relative">
                  <div className="flex items-center gap-2">
                    <DropdownMenu modal={false}>
                      <DropdownMenuTrigger render={<Button variant="outline" size="sm" />}>{filterOperationLabels[condition.operation]}</DropdownMenuTrigger>
                      <DropdownMenuContent align="start" className="w-[200px]">
                        {Object.entries(filterOperationLabels).map(([op, label]) => (
                          <DropdownMenuItem
                            key={op}
                            onClick={() => updateCondition(index, condition.value, op as FilterOperation)}
                          >
                            {label}
                          </DropdownMenuItem>
                        ))}
                      </DropdownMenuContent>
                    </DropdownMenu>
                    <Button
                      variant="ghost"
                      size="sm"
                      className="h-8 w-8 p-0"
                      onClick={() => removeCondition(index)}
                    >
                      <FilterX className="h-4 w-4" />
                    </Button>
                  </div>
                  <Input
                    placeholder={`Value for ${filterOperationLabels[condition.operation]}`}
                    value={condition.value}
                    onChange={(e) => updateCondition(index, e.target.value, condition.operation)}
                    className="h-8"
                  />
                </div>
              ))}
            </div>
          </ScrollArea>
          <div className="flex items-center justify-end gap-2 pt-4 border-t">
            <Button variant="outline" size="sm" onClick={handleClear}>Clear All</Button>
            <Button size="sm" onClick={handleApply}>Apply Filters</Button>
          </div>
        </div>
      </PopoverContent>
    </Popover>
  )
})
FilterPopover.displayName = "FilterPopover"

export default FilterPopover
