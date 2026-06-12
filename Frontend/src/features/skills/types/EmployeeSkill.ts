import type { ProficiencyLevel } from './ProficiencyLevel';

export interface EmployeeSkill {
  id: number;
  employeeId: number;
  employeeName: string;
  department: string;
  project: string;
  skillId: number;
  skillName: string;
  skillCategory: string;
  proficiency: ProficiencyLevel;
  yearsOfExperience: number;
  certification: string;
  allocationPercentage: number;
  availability: 'Available' | 'Partially Available' | 'Allocated';
}

export interface EmployeeSkillFormValues {
  employeeName: string;
  skillId: number | null;
  proficiency: ProficiencyLevel;
  yearsOfExperience: number;
  certification: string;
}
