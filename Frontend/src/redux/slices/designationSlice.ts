// Redux slice for designation CRUD operations with async API calls
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { designationService } from '../../features/designations/services/designationService';
import type { Designation, DesignationFormValues } from '../../features/designations/types/designation';

interface DesignationState {
  designations: Designation[];
  loading: boolean;
  error: string | null;
}

const initialState: DesignationState = {
  designations: [],
  loading: false,
  error: null,
};

export const fetchDesignations = createAsyncThunk('designations/fetchDesignations', designationService.getAll);

export const createDesignation = createAsyncThunk(
  'designations/createDesignation',
  async (values: DesignationFormValues) => designationService.create(values),
);

export const updateDesignation = createAsyncThunk(
  'designations/updateDesignation',
  async ({ id, values }: { id: string; values: DesignationFormValues }) =>
    designationService.update(id, values),
);

export const deleteDesignation = createAsyncThunk('designations/deleteDesignation', async (id: string) =>
  designationService.delete(id),
);

const designationSlice = createSlice({
  name: 'designations',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchDesignations.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchDesignations.fulfilled, (state, action) => {
        state.loading = false;
        state.designations = action.payload;
      })
      .addCase(fetchDesignations.rejected, (state) => {
        state.loading = false;
        state.error = 'Unable to load designations.';
      })
      .addCase(createDesignation.fulfilled, (state, action) => {
        state.designations.unshift(action.payload);
      })
      .addCase(updateDesignation.fulfilled, (state, action) => {
        state.designations = state.designations.map((d) =>
          d.id === action.payload.id ? action.payload : d,
        );
      })
      .addCase(deleteDesignation.fulfilled, (state, action) => {
        state.designations = state.designations.filter((d) => d.id !== action.payload);
      });
  },
});

export default designationSlice.reducer;
