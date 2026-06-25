export interface AllocationDto {
  id: number;
  employeeId: number;
  employeeName: string;
  employeeCode: string;
  designation: string | null;
  practice: string;
  practiceHead: string | null;
  skills: string | null;
  projectId: number;
  projectName: string;
  startDate: string;
  endDate: string | null;
  allocationPercentage: number;
  allocationStatus: string;
  allocationType: string | null;
  billableStatus: string | null;
  notes: string | null;
  totalAllocated: number;
  availableCapacity: number;
  resourceStatus: string;
}

export interface CreateAllocationDto {
  employeeId: number;
  projectId: number;
  startDate: string;
  endDate: string | null;
  allocationPercentage: number;
  allocationStatus?: string;
  allocationType?: string;
  billableStatus?: string;
  notes?: string;
}

export interface UpdateAllocationDto {
  projectId?: number;
  startDate?: string;
  endDate?: string | null;
  allocationPercentage?: number;
  allocationStatus?: string;
  allocationType?: string;
  billableStatus?: string;
  notes?: string;
}

export interface ProjectAllocationDto {
  id: number;
  projectId: number;
  projectName: string;
  clientId: number | null;
  clientName: string | null;
  startDate: string;
  endDate: string | null;
  allocationPercentage: number;
  billableStatus: string | null;
  allocationType: string | null;
  allocationStatus: string;
}

export interface AddProjectAllocationDto {
  employeeId: number;
  projectId: number;
  clientId?: number | null;
  startDate: string;
  endDate: string | null;
  allocationPercentage: number;
  billableStatus?: string;
  allocationType?: string;
  allocationStatus?: string;
}

export interface UpdateProjectAllocationDto {
  projectId?: number;
  clientId?: number | null;
  startDate?: string;
  endDate?: string | null;
  allocationPercentage?: number;
  billableStatus?: string;
  allocationType?: string;
  allocationStatus?: string;
}

export interface EmployeeAllocationDto {
  employeeId: number;
  employeeName: string;
  employeeCode: string;
  designation: string | null;
  practice: string;
  skills: string | null;
  primarySkill: string | null;
  totalExperience: number | null;
  currentUtilization: number;
  availableCapacity: number;
  resourceStatus: string;
  allocations: ProjectAllocationDto[];
}

export interface EmployeeCapacitySummaryDto {
  totalCapacity: number;
  allocatedCapacity: number;
  availableCapacity: number;
  resourceStatus: string;
}

export interface AllocationHistoryDto {
  id: number;
  allocationId: number;
  oldProject: string | null;
  newProject: string | null;
  oldAllocationPercentage: number | null;
  newAllocationPercentage: number | null;
  oldStatus: string | null;
  newStatus: string | null;
  changeType: string;
  modifiedBy: string;
  modifiedDate: string;
  remarks: string | null;
}

export interface CalendarViewDto {
  allocationId: number;
  employeeId: number;
  employeeName: string;
  projectName: string;
  startDate: string;
  endDate: string | null;
  allocationPercentage: number;
  allocationStatus: string;
  colorCode: string;
}

export interface TimelineViewDto {
  projectId: number;
  projectName: string;
  employees: TimelineEmployeeDto[];
}

export interface TimelineEmployeeDto {
  employeeId: number;
  employeeName: string;
  startDate: string;
  endDate: string | null;
  allocationPercentage: number;
  allocationStatus: string;
}

export interface ResourceAvailabilityDto {
  employeeId: number;
  employeeName: string;
  employeeCode: string;
  designation: string | null;
  practice: string;
  practiceHead: string | null;
  department: string | null;
  skills: string | null;
  currentProject: string | null;
  totalAllocated: number;
  availableCapacity: number;
  resourceStatus: string;
  nextAvailableDate: string | null;
}

export interface ResourceSuggestionDto {
  employeeId: number;
  employeeName: string;
  skillMatchPercentage: number;
  availabilityPercentage: number;
  totalAllocated: number;
  availableCapacity: number;
  resourceStatus: string;
}

export interface PracticeUtilizationDto {
  practiceId: string;
  practiceName: string;
  totalResources: number;
  allocatedResources: number;
  availableResources: number;
  benchResources: number;
  utilizationPercentage: number;
}

export interface EmployeeResourceDetailsDto {
  employeeId: number;
  employeeCode: string;
  employeeName: string;
  email: string;
  role: string | null;
  practice: string | null;
  subPractice: string | null;
  primarySkill: string | null;
  skill: string | null;
  active: boolean | null;
  location: string | null;
  l1Manager: string | null;
  practiceHead: string | null;
  doj: string | null;

  priorExperience: number;
  nvExperience: number;
  totalExperience: number;
  experienceRange: string;

  fteConsultant: string | null;
  utilised: string | null;
  billable: string | null;
  status: string | null;

  projectAllocations: ProjectAllocationDetailDto[];
}

export interface ProjectAllocationDetailDto {
  projectCode: number | null;
  client: string | null;
  project: string | null;
  projectType: string | null;
  projectStatus: string | null;
  startDate: string | null;
  endDate: string | null;
  allocationPercentage: number | null;
  billablePercentage: number | null;
  engineering: string | null;
  durationInProject: string | null;
  ageing: string | null;
  remarks: string | null;
}

export const ALLOCATION_STATUSES = ['Planned', 'Active', 'Completed', 'Released', 'Cancelled'] as const;
export const RESOURCE_STATUSES = ['Available', 'Partially Allocated', 'Fully Allocated', 'Overallocated', 'Bench', 'On Leave'] as const;
export const REQUEST_STATUSES = ['Draft', 'Submitted', 'Reviewing', 'Approved', 'Rejected', 'Fulfilled'] as const;
export const ALLOCATION_TYPES = ['Full Time', 'Part Time', 'Contract', 'Sow'] as const;
export const BILLABLE_STATUSES = ['Billable', 'Non-Billable', 'Shadow'] as const;