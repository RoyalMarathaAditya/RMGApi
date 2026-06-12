export type ProjectStatus = 'Planned' | 'Active' | 'Completed' | 'On Hold' | 'Cancelled';
export type ProjectPriority = 'Low' | 'Medium' | 'High' | 'Critical';

export interface Project {
  id: number;
  projectCode: string;
  projectName: string;
  description: string;
  clientName: string;
  clientContact: string;
  projectManager: string;
  startDate: string;
  endDate: string;
  status: ProjectStatus;
  priority: ProjectPriority;
  technologies: string[];
  allocatedResources: number;
}

export type ProjectFormValues = Omit<Project, 'id'>;

export interface ProjectState {
  projects: Project[];
  selectedProject: Project | null;
}
