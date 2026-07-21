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
  experienceRanges: ExperienceRangeDto[];
}

export interface ExperienceRangeDto {
  range: string;
  count: number;
}
