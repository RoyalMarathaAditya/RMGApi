import api from '../../../services/api';
import type { ColumnMapping, ColumnMappingFormValues, ColumnValueMapping, ColumnValueMappingFormValues } from '../types/columnMapping';

function unwrap<T>(response: { data: { success: boolean; data: T } }): T {
  return response.data.data;
}

export const columnMappingService = {
  async getAll() {
    const response = await api.get('/column-mappings');
    return unwrap(response) as ColumnMapping[];
  },

  async create(values: ColumnMappingFormValues) {
    const response = await api.post('/column-mappings', values);
    return unwrap(response) as ColumnMapping;
  },

  async update(id: string, values: ColumnMappingFormValues) {
    const response = await api.put(`/column-mappings/${id}`, values);
    return unwrap(response) as ColumnMapping;
  },

  async delete(id: string) {
    await api.delete(`/column-mappings/${id}`);
    return id;
  },
};

export const columnValueMappingService = {
  async getAll() {
    const response = await api.get('/column-value-mappings');
    return unwrap(response) as ColumnValueMapping[];
  },

  async create(values: ColumnValueMappingFormValues) {
    const response = await api.post('/column-value-mappings', values);
    return unwrap(response) as ColumnValueMapping;
  },

  async update(id: string, values: ColumnValueMappingFormValues) {
    const response = await api.put(`/column-value-mappings/${id}`, values);
    return unwrap(response) as ColumnValueMapping;
  },

  async delete(id: string) {
    await api.delete(`/column-value-mappings/${id}`);
    return id;
  },
};
