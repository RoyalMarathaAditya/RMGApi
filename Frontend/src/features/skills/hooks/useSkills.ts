import { useQuery } from '@tanstack/react-query';
import { skillService } from '../services/skillService';

export function useSkills() {
  return useQuery({
    queryFn: skillService.getSkills,
    queryKey: ['skills'],
  });
}

export function useEmployeeSkills() {
  return useQuery({
    queryFn: skillService.getEmployeeSkills,
    queryKey: ['employeeSkills'],
  });
}
