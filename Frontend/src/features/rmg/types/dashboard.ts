export interface DashboardSummaryDto {
  totalEmployees: number;
  totalPractices: number;
  availableResources: number;
  allocatedResources: number;
  fullyAllocatedResources: number;
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
  projectManager: string | null;
  allocationPercentage: number;
  availableCapacity: number;
  resourceStatus: string;
  allocationStatus: string | null;
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
