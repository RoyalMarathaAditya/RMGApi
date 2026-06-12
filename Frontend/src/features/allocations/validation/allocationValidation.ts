import { object, number, mixed, string } from 'yup';
import { allocationTypeOptions } from '../types/allocationType';

export const allocationValidationSchema = object({
  employeeId: number().required('Employee selection is required').positive('Employee selection is required'),
  projectId: number().required('Project selection is required').positive('Project selection is required'),
  allocationPercentage: number()
    .required('Allocation percentage is required')
    .min(1, 'Allocation percentage must be at least 1')
    .max(100, 'Allocation percentage cannot exceed 100'),
  allocationType: mixed()
    .oneOf(allocationTypeOptions.map((option) => option.value), 'Allocation type is required')
    .required('Allocation type is required'),
  startDate: string().required('Start date is required'),
  endDate: string()
    .required('End date is required')
    .test('is-after-start', 'End date must be after start date', function (value) {
      const { startDate } = this.parent;
      if (!startDate || !value) {
        return true;
      }
      return new Date(value) > new Date(startDate);
    }),
  isActive: mixed().oneOf([true, false]).required('Active status is required'),
});
