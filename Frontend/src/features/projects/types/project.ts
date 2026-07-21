export interface Project {
  id: number;
  projectName: string;
  projectCode?: string | null;
  clientId: number;
  clientName: string;
  projectManager?: string | null;
  deliveryHead?: string | null;
  csm?: string | null;
  csmRevenueTypeId?: string | null;
  csmRevenueTypeName?: string | null;
  projectStartDate: string;
  projectEndDate: string;
  isActive: boolean;
  description?: string | null;
}

export interface CreateProjectRequest {
  projectName: string;
  projectCode?: string | null;
  clientId: number;
  projectManager?: string | null;
  deliveryHead?: string | null;
  csm?: string | null;
  csmRevenueTypeId?: string | null;
  projectStartDate: string;
  projectEndDate: string;
  isActive: boolean;
  description?: string | null;
}

export type UpdateProjectRequest = CreateProjectRequest;
