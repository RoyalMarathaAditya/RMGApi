export interface ResourceRequestDto {
  id: number;
  projectId: number;
  projectName: string;
  requestedById: number;
  requestedByName: string;
  practiceId: string | null;
  practiceName: string | null;
  requiredSkillIds: string | null;
  requiredSkillNames: string | null;
  requiredCount: number;
  requiredByDate: string;
  status: string;
  priority: string | null;
  notes: string | null;
  createdOn: string;
  createdBy: string | null;
}

export interface CreateResourceRequestDto {
  projectId: number;
  practiceId?: string;
  requiredSkillIds?: string;
  requiredCount: number;
  requiredByDate: string;
  priority?: string;
  notes?: string;
}
