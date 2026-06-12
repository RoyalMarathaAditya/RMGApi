import type { AllocationType } from './allocationType';

export interface ResourceAllocation {
  id: number;
  employeeId: number;
  projectId: number;
  allocationPercentage: number;
  allocationType: AllocationType;
  startDate: string;
  endDate: string;
  isActive: boolean;
  createdDate: string;
  modifiedDate: string;
  employeeName?: string;
  projectName?: string;
}

export interface ResourceAllocationFormValues {
  employeeId: number;
  projectId: number;
  allocationPercentage: number;
  allocationType: AllocationType;
  startDate: string;
  endDate: string;
  isActive: boolean;
}
