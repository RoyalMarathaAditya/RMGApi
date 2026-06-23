import api from '../../../services/api';
import type { Designation, DesignationFormValues } from '../types/designation';

function unwrap<T>(response: { data: { success: boolean; data: T } }): T {
  return response.data.data;
}

export const designationService = {
  async getAll() {
    const response = await api.get('/designations');
    return unwrap(response) as Designation[];
  },

  async getActive() {
    const response = await api.get('/designations/active');
    return unwrap(response) as Designation[];
  },

  async getById(id: string) {
    const response = await api.get(`/designations/${id}`);
    return unwrap(response) as Designation;
  },

  async create(values: DesignationFormValues) {
    const response = await api.post('/designations', values);
    return unwrap(response) as Designation;
  },

  async update(id: string, values: DesignationFormValues) {
    const response = await api.put(`/designations/${id}`, {
      ...values,
      isActive: values.isActive ?? true,
    });
    return unwrap(response) as Designation;
  },

  async delete(id: string) {
    await api.delete(`/designations/${id}`);
    return id;
  },
};
