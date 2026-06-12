import * as yup from 'yup';
import { proficiencyLevels } from '../types';
import type { EmployeeSkillFormValues } from '../types';

export const employeeSkillValidationSchema: yup.ObjectSchema<EmployeeSkillFormValues> = yup.object({
  certification: yup.string().trim().max(120, 'Certification must be 120 characters or less').required(),
  employeeName: yup.string().trim().required('Employee is required'),
  proficiency: yup.mixed<EmployeeSkillFormValues['proficiency']>().oneOf(proficiencyLevels).required('Proficiency is required'),
  skillId: yup.number().nullable().required('Skill is required'),
  yearsOfExperience: yup.number().min(0, 'Experience cannot be negative').max(50, 'Experience must be realistic').required('Experience is required'),
});
