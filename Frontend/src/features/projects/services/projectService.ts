import api from '../../../services/api';
import type { Project, CreateProjectRequest } from '../types/project';

export const projectService = {
  async getAll() {
    const { data } = await api.get('/projects');
    return data as Project[];
  },

  async getById(id: number) {
    const { data } = await api.get(`/projects/${id}`);
    return data as Project;
  },

  async create(values: CreateProjectRequest) {
    const { data } = await api.post('/projects', values);
    return data as Project;
  },

  async update(id: number, values: CreateProjectRequest) {
    const { data } = await api.put(`/projects/${id}`, values);
    return data as Project;
  },

  async delete(id: number) {
    await api.delete(`/projects/${id}`);
    return id;
  },

  async getClients() {
    const response = await api.get('/clients');
    return response.data.data as Array<{ id: number; name: string }>;
  },

  async getRevenueTypes() {
    const { data } = await api.get('/master/csmrevengetypes');
    return data as Array<{ id: string; name: string }>;
  },
};
