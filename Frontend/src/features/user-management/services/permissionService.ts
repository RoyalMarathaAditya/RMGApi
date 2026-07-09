import api from '../../../services/api';
import type { PermissionDto, RolePermissionDto } from '../types/userManagement';

function unwrap<T>(response: { data: T }): T {
  return response.data;
}

export const permissionService = {
  async getAllPermissions(): Promise<PermissionDto[]> {
    const response = await api.get<PermissionDto[]>('/permissions');
    return unwrap(response);
  },

  async getPermissionsByRole(roleName: string): Promise<PermissionDto[]> {
    const response = await api.get<PermissionDto[]>(`/permissions/by-role/${encodeURIComponent(roleName)}`);
    return unwrap(response);
  },

  async saveRolePermissions(dto: RolePermissionDto): Promise<{ success: boolean; message: string }> {
    const response = await api.put('/permissions/role-permissions', dto);
    return unwrap(response);
  },
};