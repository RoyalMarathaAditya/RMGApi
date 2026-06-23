// Redux slice for project state (uses mock data, no async thunks)
import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import { mockProjects } from '../../features/projects/mock/projects';
import type { Project, ProjectFormValues, ProjectState } from '../../features/projects/types/project.types';

const initialState: ProjectState = {
  projects: mockProjects,
  selectedProject: null,
};

const projectSlice = createSlice({
  name: 'projects',
  initialState,
  reducers: {
    addProject: (state, action: PayloadAction<ProjectFormValues>) => {
      const nextId = Math.max(0, ...state.projects.map((project) => project.id)) + 1;
      state.projects.unshift({
        id: nextId,
        ...action.payload,
      });
    },
    updateProject: (state, action: PayloadAction<Project>) => {
      const index = state.projects.findIndex((project) => project.id === action.payload.id);

      if (index >= 0) {
        state.projects[index] = action.payload;
      }
    },
    deleteProject: (state, action: PayloadAction<number>) => {
      state.projects = state.projects.filter((project) => project.id !== action.payload);

      if (state.selectedProject?.id === action.payload) {
        state.selectedProject = null;
      }
    },
    setSelectedProject: (state, action: PayloadAction<Project | null>) => {
      state.selectedProject = action.payload;
    },
  },
});

export const { addProject, deleteProject, setSelectedProject, updateProject } = projectSlice.actions;
export default projectSlice.reducer;
