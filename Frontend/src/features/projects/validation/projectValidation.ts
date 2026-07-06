import * as yup from 'yup';
import type { ProjectFormValues } from '../types/project.types';

export const projectValidationSchema: yup.ObjectSchema<ProjectFormValues> = yup.object({
  projectCode: yup.string().trim().required('Project code is required'),
  projectName: yup.string().trim().required('Project name is required'),
  description: yup.string().trim().required('Description is required'),
  clientName: yup.string().trim().required('Client name is required'),
  clientContact: yup.string().trim().required('Client contact is required'),
  projectManager: yup.string().trim().required('Project manager is required'),
  startDate: yup.string().required('Start date is required'),
  endDate: yup
    .string()
    .required('End date is required')
    .test('is-after-start', 'End date must be on or after the start date', function validateEndDate(value) {
      const { startDate } = this.parent as ProjectFormValues;

      if (!startDate || !value) {
        return true;
      }

      return new Date(value) >= new Date(startDate);
    }),
  status: yup.mixed<ProjectFormValues['status']>().required('Status is required'),
  priority: yup.mixed<ProjectFormValues['priority']>().required('Priority is required'),
  technologies: yup.array().of(yup.string().required()).min(1, 'Select at least one technology').required(),
  allocatedResources: yup
    .number()
    .typeError('Allocated resources must be a number')
    .integer('Allocated resources must be a whole number')
    .min(0, 'Allocated resources cannot be negative')
    .required('Allocated resources is required'),
  csm: yup.string().trim(),
});
