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
  billableStatus?: string;
  notes?: string;
}

export interface UpdateAllocationDto {
  projectId?: number;
  startDate?: string;
  endDate?: string | null;
  allocationPercentage?: number;
  allocationStatus?: string;
  billableStatus?: string;
  notes?: string;
}

export interface ProjectAllocationDto {
  id: number;
  projectId: number;
  projectName: string;
  clientId: number | null;
  clientName: string | null;
  projectStatusId: string | null;
  statusId: string | null;
  probableNextAssignmentId: string | null;
  probableNextAssignmentDate: string | null;
  billableDateProbabilityId: string | null;
  currentBillingStatusId: string | null;
  billingBucketId: string | null;
  ageingBucketId: string | null;
  actionItem: string | null;
  remarks: string | null;
  startDate: string;
  endDate: string | null;
  allocationPercentage: number;
  billableStatus: string | null;
  allocationStatus: string;
  engineering: string | null;
}

export interface AddProjectAllocationDto {
  employeeId: number;
  projectId: number;
  clientId?: number | null;
  projectStatusId?: string | null;
  statusId?: string | null;
  probableNextAssignmentId?: string | null;
  probableNextAssignmentDate?: string | null;
  billableDateProbabilityId?: string | null;
  currentBillingStatusId?: string | null;
  billingBucketId?: string | null;
  actionItem?: string | null;
  remarks?: string | null;
  startDate: string;
  endDate: string | null;
  allocationPercentage: number;
  billableStatus?: string;
  allocationStatus?: string;
  engineering?: string;
}

export interface UpdateProjectAllocationDto {
  projectId?: number;
  clientId?: number | null;
  projectStatusId?: string | null;
  statusId?: string | null;
  probableNextAssignmentId?: string | null;
  probableNextAssignmentDate?: string | null;
  billableDateProbabilityId?: string | null;
  currentBillingStatusId?: string | null;
  billingBucketId?: string | null;
  ageingBucketId?: string | null;
  actionItem?: string | null;
  remarks?: string | null;
  startDate?: string;
  endDate?: string | null;
  allocationPercentage?: number;
  billableStatus?: string;
  allocationStatus?: string;
  engineering?: string;
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
  relevantExperience: number | null;
  isActive: boolean;
  reportingManagerId: number | null;
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

export interface ApiProject {
  id: number;
  projectName: string;
  projectCode: string | null;
  clientId: number;
  clientName: string;
  isActive: boolean;
  projectManager: string | null;
  deliveryHead: string | null;
  csm: string | null;
  csmRevenueTypeName: string | null;
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

  projectManagerId: number | null;
  projectManagerName: string | null;
  remarks: string | null;

  projectAllocations: ProjectAllocationDetailDto[];
}

export interface ProjectAllocationDetailDto {
  projectCode: string | null;
  client: string | null;
  project: string | null;
  projectManager: string | null;
  deliveryHead: string | null;
  csm: string | null;
  projectStatus: string | null;
  allocationStatus: string | null;
  startDate: string | null;
  endDate: string | null;
  allocationPercentage: number | null;
  billablePercentage: number | null;
  billableStatus: string | null;
  status: string | null;
  currentBillingStatus: string | null;
  billableDateProbability: string | null;
  billingBucket: string | null;
  durationInProject: string | null;
  ageing: string | null;
  ageingBucket: string | null;
  probableNextAssignment: string | null;
  probableNextAssignmentDate: string | null;
  engineering: string | null;
  actionItem: string | null;
  remarks: string | null;
  notes: string | null;
}

export interface BulkProjectAllocationDto {
  employeeIds: number[];
  projectId: number;
  clientId?: number | null;
  projectStatusId?: string | null;
  statusId?: string | null;
  probableNextAssignmentId?: string | null;
  probableNextAssignmentDate?: string | null;
  billableDateProbabilityId?: string | null;
  currentBillingStatusId?: string | null;
  billingBucketId?: string | null;
  actionItem?: string | null;
  remarks?: string | null;
  startDate: string;
  endDate: string | null;
  allocationPercentage: number;
  billableStatus?: string;
  allocationStatus?: string;
  engineering?: string;
}

export const ALLOCATION_STATUSES = ['Current', 'History', 'Planned', 'Active', 'Completed', 'Released', 'Cancelled'] as const;
export const RESOURCE_STATUSES = ['Available', 'Partially Allocated', 'Fully Allocated', 'Overallocated', 'Bench', 'On Leave'] as const;
export const REQUEST_STATUSES = ['Draft', 'Submitted', 'Reviewing', 'Approved', 'Rejected', 'Fulfilled'] as const;
export const BILLABLE_STATUSES = ['Billable', 'Non-Billable', 'Shadow'] as const;