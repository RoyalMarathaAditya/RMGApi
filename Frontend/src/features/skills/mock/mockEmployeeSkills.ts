import { mockSkills } from './mockSkills';
import type { EmployeeSkill } from '../types';

const employees = [
  ['Aarav Mehta', 'Engineering', 'Phoenix'],
  ['Isha Rao', 'Engineering', 'Atlas'],
  ['Kabir Shah', 'Data', 'Insight'],
  ['Maya Iyer', 'Design', 'Nova'],
  ['Neha Kapoor', 'QA', 'Phoenix'],
  ['Rohan Das', 'Platform', 'Atlas'],
  ['Sara Khan', 'Security', 'Sentinel'],
  ['Vikram Nair', 'Engineering', 'Nova'],
  ['Ananya Sen', 'Business', 'Mercury'],
  ['Dev Patel', 'Mobile', 'Orion'],
] as const;

const proficiencies = ['Beginner', 'Intermediate', 'Advanced', 'Expert'] as const;
const availability = ['Available', 'Partially Available', 'Allocated'] as const;

export const mockEmployeeSkills: EmployeeSkill[] = Array.from({ length: 60 }, (_, index) => {
  const employee = employees[index % employees.length];
  const skill = mockSkills[(index * 7) % mockSkills.length];
  const proficiency = proficiencies[index % proficiencies.length];

  return {
    id: index + 1,
    employeeId: (index % employees.length) + 1,
    employeeName: employee[0],
    department: employee[1],
    project: employee[2],
    skillId: skill.id,
    skillName: skill.skillName,
    skillCategory: skill.category,
    proficiency,
    yearsOfExperience: Number(((index % 9) + 0.5).toFixed(1)),
    certification: index % 3 === 0 ? `${skill.skillName} Certified` : '',
    allocationPercentage: [40, 60, 80, 100][index % 4],
    availability: availability[index % availability.length],
  };
});
