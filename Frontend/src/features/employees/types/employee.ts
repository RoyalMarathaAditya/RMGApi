
export interface Employee {
  id: number;
  employeeCode: string;
  fullName: string;
  email: string;
  doj: string;
  lwd?: string | null;
  priorExperience: number;
  relevantExperience?: number | null;
  mobileNumber?: string | null;
  deloitteFitment?: boolean | null;
  engineering?: boolean | null;
  employmentTypeId: string;
  employmentType: string;
  locationId: string;
  location: string;
  workModelId: string;
  workModel: string;
  practiceId: string;
  practice: string;
  departmentTypeId: string;
  departmentType: string;
  statusId: string;
  employeeStatus: string;
  reportingManagerName?: string | null;
  practiceHeadName?: string | null;
  designationId?: string | null;
  designation?: string | null;
  subPracticeId?: string | null;
  subPractice?: string | null;
  skills: Array<{ id: string; name: string }>;
}

export interface EmployeeFormValues {
  employeeCode: string;
  fullName: string;
  email: string;
  doj: string;
  lwd?: string | null;
  priorExperience: number;
  relevantExperience?: number | null;
  employmentTypeId: string;
  locationId: string;
  workModelId: string;
  practiceId: string;
  departmentTypeId: string;
  statusId: string;
  reportingManagerName?: string | null;
  practiceHeadName?: string | null;
  designationId?: string | null;
  subPracticeId?: string | null;
  deloitteFitment?: boolean | null;
  engineering?: boolean | null;
  mobileNumber?: string | null;
  skillIds: string[];
}

export interface EmployeeFilters {
  search: string;
  status: string;
}
