import api from '../../../services/api';
import type { ResourceRequestDto, CreateResourceRequestDto } from '../types/request';

function unwrap<T>(response: { data: T }): T {
  return response.data;
}

export const requestService = {
  async getAll(): Promise<ResourceRequestDto[]> {
    const response = await api.get<ResourceRequestDto[]>('/resource-requests');
    return unwrap(response);
  },

  async getById(id: number): Promise<ResourceRequestDto> {
    const response = await api.get<ResourceRequestDto>(`/resource-requests/${id}`);
    return unwrap(response);
  },

  async create(values: CreateResourceRequestDto): Promise<ResourceRequestDto> {
    const response = await api.post<ResourceRequestDto>('/resource-requests', values);
    return unwrap(response);
  },

  async updateStatus(id: number, status: string, notes?: string): Promise<ResourceRequestDto> {
    const response = await api.put<ResourceRequestDto>(`/resource-requests/${id}/status`, { status, notes });
    return unwrap(response);
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/resource-requests/${id}`);
  },
};
