import api from '../../../services/api';
import type {
  UserListDto,
  CreateUserDto,
  UpdateUserDto,
  ResetPasswordDto,
  PagedResponse,
  PaginationParams,
  AvailableEmployee,
  EmployeeDetail,
} from '../types/userManagement';

function unwrap<T>(response: { data: T }): T {
  return response.data;
}

export const userService = {
  async getUsers(params: PaginationParams): Promise<PagedResponse<UserListDto>> {
    const query = new URLSearchParams();
    query.set('pageNumber', String(params.pageNumber));
    query.set('pageSize', String(params.pageSize));
    if (params.searchTerm) query.set('searchTerm', params.searchTerm);
    if (params.sortBy) query.set('sortBy', params.sortBy);
    if (params.sortDescending) query.set('sortDescending', 'true');
    if (params.roleFilter) query.set('roleFilter', params.roleFilter);
    if (params.statusFilter) query.set('statusFilter', params.statusFilter);
    const response = await api.get<PagedResponse<UserListDto>>(`/users?${query.toString()}`);
    return unwrap(response);
  },

  async getUserById(id: number): Promise<UserListDto> {
    const response = await api.get<UserListDto>(`/users/${id}`);
    return unwrap(response);
  },

  async getAvailableEmployees(): Promise<AvailableEmployee[]> {
    const response = await api.get<AvailableEmployee[]>('/users/available-employees');
    return unwrap(response);
  },

  async createUser(dto: CreateUserDto): Promise<{ success: boolean; message: string; data?: UserListDto }> {
    const response = await api.post('/users', dto);
    return unwrap(response);
  },

  async updateUser(id: number, dto: UpdateUserDto): Promise<{ success: boolean; message: string; data?: UserListDto }> {
    const response = await api.put(`/users/${id}`, dto);
    return unwrap(response);
  },

  async deleteUser(id: number): Promise<{ success: boolean; message: string }> {
    const response = await api.delete(`/users/${id}`);
    return unwrap(response);
  },

  async resetPassword(dto: ResetPasswordDto): Promise<{ success: boolean; message: string }> {
    const response = await api.post('/users/reset-password', dto);
    return unwrap(response);
  },

  async lockUser(id: number): Promise<{ success: boolean; message: string }> {
    const response = await api.post(`/users/${id}/lock`);
    return unwrap(response);
  },

  async unlockUser(id: number): Promise<{ success: boolean; message: string }> {
    const response = await api.post(`/users/${id}/unlock`);
    return unwrap(response);
  },

  async activateUser(id: number): Promise<{ success: boolean; message: string }> {
    const response = await api.post(`/users/${id}/activate`);
    return unwrap(response);
  },

  async deactivateUser(id: number): Promise<{ success: boolean; message: string }> {
    const response = await api.post(`/users/${id}/deactivate`);
    return unwrap(response);
  },

  async getEmployeeDetail(employeeId: number): Promise<EmployeeDetail> {
    const response = await api.get<any>(`/employees/${employeeId}`);
    return response.data.data as EmployeeDetail;
  },
};