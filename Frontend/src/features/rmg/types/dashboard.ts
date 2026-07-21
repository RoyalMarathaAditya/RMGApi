export interface DashboardSummaryDto {
  totalEmployees: number;
  totalPractices: number;
  availableResources: number;
  partiallyAllocatedResources: number;
  fullyAllocatedResources: number;
  totalAllocatedResources: number;
  overallocatedResources: number;
  benchResources: number;
  resourcesOnLeave: number;
  practiceUtilizationPercentage: number;
}

export interface DashboardGridDto {
  employeeId: number;
  employeeName: string;
  employeeCode: string;
  designation: string | null;
  department: string | null;
  practice: string;
  practiceHead: string | null;
  subPractice: string | null;
  totalExperience: number;
  skills: string | null;
  currentProject: string | null;
  projects: string[];
  allocationPercentage: number;
  availableCapacity: number;
  resourceStatus: string;
  allocationStatus: string | null;
  isActive: boolean;
}

export interface DashboardFilterDto {
  searchTerm?: string;
  employeeName?: string;
  employeeCode?: string;
  department?: string;
  practice?: string;
  practiceHead?: string;
  skill?: string;
  designation?: string;
  project?: string;
  allocationStatus?: string;
  resourceStatus?: string;
  dateFrom?: string;
  dateTo?: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface GridQueryParams {
  page?: number;
  pageSize?: number;
  sortField?: string;
  sortDirection?: string;
  searchTerm?: string;
  practice?: string;
  resourceStatus?: string;
  designation?: string;
  department?: string;
}
