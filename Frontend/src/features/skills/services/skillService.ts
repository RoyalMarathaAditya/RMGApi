import axios from 'axios';
import type { EmployeeSkill, Skill } from '../types';

const client = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ? `${import.meta.env.VITE_API_BASE_URL}/skills` : '/api/skills',
  timeout: 5000,
});

export const skillService = {
  async getEmployeeSkills(employeeId: number): Promise<EmployeeSkill[]> {
    const res = await client.get(`/by-employee/${employeeId}`);
    return res.data as EmployeeSkill[];
  },
  async getSkillById(id: number): Promise<Skill> {
    const res = await client.get(`/${id}`);
    return res.data as Skill;
  },
  async getSkills(): Promise<Skill[]> {
    const res = await client.get('/');
    return res.data as Skill[];
  },
};
