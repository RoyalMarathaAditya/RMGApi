import type { ProficiencyLevel } from './ProficiencyLevel';

export interface SkillMatrixEmployee {
  employeeId: number;
  employeeName: string;
  department: string;
  project: string;
  skills: Record<number, ProficiencyLevel | null>;
}

export interface SkillMatrixFilters {
  department: string;
  project: string;
  category: string;
}
