import api from '../../../services/api';
import type { Client, CreateClientRequest, UpdateClientRequest } from '../types/client';

function unwrap<T>(response: { data: { success: boolean; data: T } }): T {
  return response.data.data;
}

export const clientService = {
  async getAll() {
    const response = await api.get('/clients');
    return unwrap(response) as Client[];
  },

  async getById(id: number) {
    const response = await api.get(`/clients/${id}`);
    return unwrap(response) as Client;
  },

  async create(values: CreateClientRequest) {
    const response = await api.post('/clients', values);
    return unwrap(response) as Client;
  },

  async update(id: number, values: UpdateClientRequest) {
    const response = await api.put(`/clients/${id}`, values);
    return unwrap(response) as Client;
  },

  async delete(id: number) {
    const response = await api.delete(`/clients/${id}`);
    return unwrap(response) as boolean;
  },

  async getStatuses() {
    const response = await api.get('/master/statuses');
    return response.data as Array<{ id: string; name: string }>;
  },
};
