import type { ResourceAllocation } from '../types/resourceAllocation';
import type { BenchResource } from '../types/benchResource';
import type { BillableResource } from '../types/billableResource';
import type { ResourceUtilization } from '../types/resourceUtilization';

export const allocationData: ResourceAllocation[] = [
  {
    id: 1,
    employeeId: 1,
    projectId: 1,
    allocationPercentage: 80,
    allocationType: 1,
    startDate: '2026-06-01',
    endDate: '2026-12-31',
    isActive: true,
    createdDate: '2026-06-01',
    modifiedDate: '2026-06-01',
    employeeName: 'Jane Doe',
    projectName: 'Apollo',
  },
  {
    id: 2,
    employeeId: 2,
    projectId: 2,
    allocationPercentage: 60,
    allocationType: 1,
    startDate: '2026-06-01',
    endDate: '2026-08-31',
    isActive: true,
    createdDate: '2026-06-01',
    modifiedDate: '2026-06-01',
    employeeName: 'John Smith',
    projectName: 'Beacon',
  },
  {
    id: 3,
    employeeId: 3,
    projectId: 1,
    allocationPercentage: 50,
    allocationType: 2,
    startDate: '2026-06-01',
    endDate: '2026-07-31',
    isActive: true,
    createdDate: '2026-06-01',
    modifiedDate: '2026-06-01',
    employeeName: 'Sarah Johnson',
    projectName: 'Apollo',
  },
];

export const benchResourceData: BenchResource[] = [
  {
    id: 1,
    employeeId: 4,
    employeeName: 'Mike Chen',
    department: 'Engineering',
    benchDays: 15,
    lastProject: 'Apollo',
    availabilityDate: '2026-06-15',
  },
  {
    id: 2,
    employeeId: 5,
    employeeName: 'Lisa Anderson',
    department: 'Design',
    benchDays: 30,
    lastProject: 'Beacon',
    availabilityDate: '2026-06-20',
  },
];

export const billableResourceData: BillableResource[] = [
  {
    id: 1,
    employeeId: 1,
    employeeName: 'Jane Doe',
    projectName: 'Apollo',
    allocationPercentage: 80,
    startDate: '2026-06-01',
    endDate: '2026-12-31',
  },
  {
    id: 2,
    employeeId: 2,
    employeeName: 'John Smith',
    projectName: 'Beacon',
    allocationPercentage: 60,
    startDate: '2026-06-01',
    endDate: '2026-08-31',
  },
];

export const utilizationData: ResourceUtilization[] = [
  { period: '2026-04', utilizationRate: 78, billableRate: 65, benchRate: 22 },
  { period: '2026-05', utilizationRate: 82, billableRate: 70, benchRate: 18 },
  { period: '2026-06', utilizationRate: 85, billableRate: 72, benchRate: 15 },
];
