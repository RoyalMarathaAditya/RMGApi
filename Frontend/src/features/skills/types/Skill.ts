import type { SkillCategory } from './SkillCategory';
import type { SkillStatus } from './SkillStatus';

export interface Skill {
  id: number;
  skillCode: string;
  skillName: string;
  description: string;
  category: SkillCategory;
  status: SkillStatus;
  employeeCount: number;
}

export type SkillFormValues = Omit<Skill, 'id' | 'employeeCount'>;
