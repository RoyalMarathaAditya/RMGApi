import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { projectService } from '../../features/projects/services/projectService';
import type { Project, CreateProjectRequest } from '../../features/projects/types/project';

interface ProjectManagementState {
  projects: Project[];
  clients: Array<{ id: number; name: string }>;
  revenueTypes: Array<{ id: string; name: string }>;
  loading: boolean;
  error: string | null;
}

const initialState: ProjectManagementState = {
  projects: [],
  clients: [],
  revenueTypes: [],
  loading: false,
  error: null,
};

export const fetchProjects = createAsyncThunk('projectManagement/fetchProjects', projectService.getAll);

export const fetchClients = createAsyncThunk('projectManagement/fetchClients', projectService.getClients);

export const fetchRevenueTypes = createAsyncThunk('projectManagement/fetchRevenueTypes', projectService.getRevenueTypes);

export const createProject = createAsyncThunk(
  'projectManagement/createProject',
  async (values: CreateProjectRequest) => projectService.create(values),
);

export const updateProject = createAsyncThunk(
  'projectManagement/updateProject',
  async ({ id, values }: { id: number; values: CreateProjectRequest }) =>
    projectService.update(id, values),
);

export const deleteProject = createAsyncThunk(
  'projectManagement/deleteProject',
  async (id: number) => {
    await projectService.delete(id);
    return id;
  },
);

const projectManagementSlice = createSlice({
  name: 'projectManagement',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchProjects.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchProjects.fulfilled, (state, action) => {
        state.loading = false;
        state.projects = action.payload;
      })
      .addCase(fetchProjects.rejected, (state) => {
        state.loading = false;
        state.error = 'Unable to load projects.';
      })
      .addCase(fetchClients.fulfilled, (state, action) => {
        state.clients = action.payload;
      })
      .addCase(fetchRevenueTypes.fulfilled, (state, action) => {
        state.revenueTypes = action.payload;
      })
      .addCase(createProject.fulfilled, (state, action) => {
        state.projects.unshift(action.payload);
      })
      .addCase(updateProject.fulfilled, (state, action) => {
        state.projects = state.projects.map((p) =>
          p.id === action.payload.id ? action.payload : p,
        );
      })
      .addCase(deleteProject.fulfilled, (state, action) => {
        state.projects = state.projects.filter((p) => p.id !== action.payload);
      });
  },
});

export default projectManagementSlice.reducer;
