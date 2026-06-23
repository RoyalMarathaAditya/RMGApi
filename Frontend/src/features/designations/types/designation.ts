export interface Designation {
  id: string;
  name: string;
  code: string;
  isActive: boolean;
  sortOrder: number;
  createdOn: string;
  modifiedOn?: string | null;
}

export interface DesignationFormValues {
  name: string;
  code: string;
  sortOrder: number;
  isActive?: boolean;
}
