export interface ColumnMapping {
  id: string;
  sourceColumn: string;
  targetProperty: string;
  targetDisplayName: string;
  dataType: string;
  isRequired: boolean;
  isActive: boolean;
  displayOrder: number;
  createdOn: string;
  modifiedOn?: string | null;
}

export interface ColumnMappingFormValues {
  sourceColumn: string;
  targetProperty: string;
  targetDisplayName: string;
  dataType: string;
  isRequired: boolean;
  isActive: boolean;
  displayOrder: number;
}

export interface ColumnValueMapping {
  id: string;
  targetProperty: string;
  sourceValue: string;
  targetValue: string;
  isActive: boolean;
  createdOn: string;
  modifiedOn?: string | null;
}

export interface ColumnValueMappingFormValues {
  targetProperty: string;
  sourceValue: string;
  targetValue: string;
  isActive: boolean;
}
