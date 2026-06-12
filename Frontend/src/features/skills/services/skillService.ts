import axios from 'axios';
import { mockEmployeeSkills } from '../mock/mockEmployeeSkills';
import { mockSkills } from '../mock/mockSkills';
import type { EmployeeSkill, Skill } from '../types';

const skillClient = axios.create({
  baseURL: '/local-mock/skills',
  timeout: 500,
});

const wait = async () => new Promise((resolve) => window.setTimeout(resolve, 250));

export const skillService = {
  client: skillClient,
  async getEmployeeSkills(): Promise<EmployeeSkill[]> {
    await wait();
    return mockEmployeeSkills;
  },
  async getSkillById(id: number): Promise<Skill | undefined> {
    await wait();
    return mockSkills.find((skill) => skill.id === id);
  },
  async getSkills(): Promise<Skill[]> {
    await wait();
    return mockSkills;
  },
};
