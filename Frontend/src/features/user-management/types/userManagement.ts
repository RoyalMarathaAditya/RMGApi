export interface UserListDto {
  id: number;
  name: string;
  userName: string | null;
  email: string;
  phone: string | null;
  role: string;
  employeeId: number | null;
  employeeCode: string | null;
  employeeName: string | null;
  designation: string | null;
  practice: string | null;
  department: string | null;
  isActive: boolean;
  isLocked: boolean;
  lastLoginDate: string | null;
  createdAt: string;
  createdBy: string | null;
  modifiedBy: string | null;
  modifiedOn: string | null;
  failedLoginCount: number;
  lockedDate: string | null;
  lockedBy: string | null;
}

export interface CreateUserDto {
  employeeId: number | null;
  userName: string;
  name: string;
  email: string;
  phone: string;
  password: string;
  confirmPassword: string;
  role: string;
  isActive: boolean;
}

export interface UpdateUserDto {
  phone?: string;
  email?: string;
  role?: string;
  isActive?: boolean;
}

export interface ResetPasswordDto {
  userId: number;
  newPassword: string;
  confirmPassword: string;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface PaginationParams {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
  sortBy?: string;
  sortDescending?: boolean;
  roleFilter?: string;
  statusFilter?: string;
}

export interface RoleDto {
  id: string;
  name: string;
  description: string | null;
  isActive: boolean;
  userCount: number;
  createdOn: string;
}

export interface CreateRoleDto {
  name: string;
  description: string;
  isActive: boolean;
}

export interface PermissionDto {
  id: number;
  name: string;
  description: string | null;
  category: string | null;
  isActive: boolean;
  hasPermission: boolean;
}

export interface RolePermissionDto {
  roleName: string;
  permissionIds: number[];
}

export interface AvailableEmployee {
  id: number;
  fullName: string;
  employeeCode: string;
  email: string;
}

export interface EmployeeDetail {
  id: number;
  employeeCode: string;
  fullName: string;
  email: string;
  location: string;
  departmentType: string;
  designation: string;
  employeeStatus: string;
  reportingManagerName: string | null;
  practiceHeadName: string | null;
  mobileNumber: string | null;
  employmentType: string;
  practice: string;
  doj: string;
}