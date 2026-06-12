export interface BillableResource {
  id: number;
  employeeId: number;
  employeeName: string;
  projectName: string;
  allocationPercentage: number;
  startDate: string;
  endDate: string;
}
