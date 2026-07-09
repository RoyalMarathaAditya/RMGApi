import api from '../../../services/api';
import type { RoleDto, CreateRoleDto } from '../types/userManagement';

function unwrap<T>(response: { data: T }): T {
  return response.data;
}

export const roleService = {
  async getRoles(): Promise<RoleDto[]> {
    const response = await api.get<RoleDto[]>('/roles');
    return unwrap(response);
  },

  async getRoleById(id: string): Promise<RoleDto> {
    const response = await api.get<RoleDto>(`/roles/${id}`);
    return unwrap(response);
  },

  async createRole(dto: CreateRoleDto): Promise<{ success: boolean; message: string; data?: RoleDto }> {
    const response = await api.post('/roles', dto);
    return unwrap(response);
  },

  async updateRole(id: string, dto: Partial<CreateRoleDto>): Promise<{ success: boolean; message: string; data?: RoleDto }> {
    const response = await api.put(`/roles/${id}`, dto);
    return unwrap(response);
  },

  async deleteRole(id: string): Promise<{ success: boolean; message: string }> {
    const response = await api.delete(`/roles/${id}`);
    return unwrap(response);
  },

  async activateRole(id: string): Promise<{ success: boolean; message: string }> {
    const response = await api.post(`/roles/${id}/activate`);
    return unwrap(response);
  },

  async deactivateRole(id: string): Promise<{ success: boolean; message: string }> {
    const response = await api.post(`/roles/${id}/deactivate`);
    return unwrap(response);
  },
};