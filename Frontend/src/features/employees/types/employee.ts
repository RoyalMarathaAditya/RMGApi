
export type EmployeeStatus = 'Active' | 'Inactive' | 'On Leave';
export type EmployeeRole = 'Admin' | 'HR' | 'Manager' | 'Team Lead' | 'Employee';

export interface Employee {
  id: number;
  employeeCode: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  department: string;
  designation: string;
  role: EmployeeRole;
  skillIds: number[];
  experience: number;
  joiningDate: string;
  status: EmployeeStatus;
}

export type EmployeeFormValues = Omit<Employee, 'id'>;

export interface EmployeeFilters {
  search: string;
  status: EmployeeStatus | 'All';
}
