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
  reportingManagerId?: number | null;
  notes?: string;
}

export interface UpdateAllocationDto {
  projectId?: number;
  startDate?: string;
  endDate?: string | null;
  allocationPercentage?: number;
  allocationStatus?: string;
  notes?: string;
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

export const ALLOCATION_STATUSES = ['Planned', 'Active', 'Completed', 'Released', 'Cancelled'] as const;
export const RESOURCE_STATUSES = ['Available', 'Partially Allocated', 'Fully Allocated', 'Overallocated', 'Bench', 'On Leave'] as const;
export const REQUEST_STATUSES = ['Draft', 'Submitted', 'Reviewing', 'Approved', 'Rejected', 'Fulfilled'] as const;
