export interface PracticeWiseReportDto {
  practiceId: string;
  practiceName: string;
  totalHeadcount: number;
  billableCount: number;
  nonBillableCount: number;
  utilizedCount: number;
  nonUtilizedCount: number;
  billabilityPercentage: number;
  utilizationPercentage: number;
  engineeringHeadcount: number;
  experienceRanges: ExperienceRangeDto[];
}

export interface ExperienceRangeDto {
  range: string;
  count: number;
}

// Client Wise Report
export interface ClientWiseReportDto {
  clientId: number;
  clientName: string;
  totalProjects: number;
  activeProjects: number;
  completedProjects: number;
  totalEmployees: number;
  billableCount: number;
  nonBillableCount: number;
  avgAllocation: number;
  projects: ClientProjectDto[];
}

export interface ClientProjectDto {
  projectId: number;
  projectName: string;
  status: string;
  employeeCount: number;
  avgAllocation: number;
}

// Project Wise Report
export interface ProjectWiseReportDto {
  projectId: number;
  projectName: string;
  projectCode: string | null;
  clientName: string;
  projectManager: string | null;
  projectStartDate: string;
  projectEndDate: string | null;
  totalEmployees: number;
  billableCount: number;
  avgAllocation: number;
  status: string;
  employees: ProjectEmployeeDto[];
}

export interface ProjectEmployeeDto {
  employeeCode: string;
  fullName: string;
  allocationPercentage: number;
  billableStatus: string | null;
}

// Chart Data
export interface ReportChartDataDto {
  billableDistribution: DonutDataDto[];
  utilizationByEntity: BarDataDto[];
  projectsByStatus: PieDataDto[];
  monthlyTrend: LineDataDto[];
}

export interface DonutDataDto {
  name: string;
  value: number;
}

export interface BarDataDto {
  name: string;
  utilization: number;
}

export interface PieDataDto {
  name: string;
  count: number;
}

export interface LineDataDto {
  month: string;
  started: number;
  ended: number;
}
