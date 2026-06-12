import * as yup from 'yup';
import { skillCategories, skillStatuses } from '../types';
import type { SkillFormValues } from '../types';

export const skillValidationSchema: yup.ObjectSchema<SkillFormValues> = yup.object({
  category: yup.mixed<SkillFormValues['category']>().oneOf(skillCategories).required('Category is required'),
  description: yup.string().trim().required('Description is required').max(500, 'Description must be 500 characters or less'),
  skillCode: yup.string().trim().required('Skill code is required').max(20, 'Skill code must be 20 characters or less'),
  skillName: yup.string().trim().required('Skill name is required').max(80, 'Skill name must be 80 characters or less'),
  status: yup.mixed<SkillFormValues['status']>().oneOf(skillStatuses).required('Status is required'),
});
