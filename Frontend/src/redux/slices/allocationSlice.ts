// Redux slice for resource allocations, bench/billable resources, and utilization data
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { allocationService } from '../../features/allocations/services/allocationService';
import type { BenchResource } from '../../features/allocations/types/benchResource';
import type { BillableResource } from '../../features/allocations/types/billableResource';
import type { ResourceAllocation, ResourceAllocationFormValues } from '../../features/allocations/types/resourceAllocation';
import type { ResourceUtilization } from '../../features/allocations/types/resourceUtilization';

interface AllocationState {
  allocations: ResourceAllocation[];
  benchResources: BenchResource[];
  billableResources: BillableResource[];
  utilization: ResourceUtilization[];
  loading: boolean;
  error: string | null;
}

export type { AllocationState };

const initialState: AllocationState = {
  allocations: [],
  benchResources: [],
  billableResources: [],
  utilization: [],
  loading: false,
  error: null,
};

export const fetchAllocations = createAsyncThunk('allocations/fetchAllocations', async () => allocationService.getAll());
export const createAllocation = createAsyncThunk(
  'allocations/createAllocation',
  async (values: ResourceAllocationFormValues) => allocationService.create(values),
);
export const updateAllocation = createAsyncThunk(
  'allocations/updateAllocation',
  async ({ id, values }: { id: number; values: ResourceAllocationFormValues }) =>
    allocationService.update(id, values),
);
export const deleteAllocation = createAsyncThunk('allocations/deleteAllocation', async (id: number) =>
  allocationService.delete(id),
);
export const fetchBenchResources = createAsyncThunk('allocations/fetchBenchResources', async () =>
  allocationService.getBenchResources(),
);
export const fetchBillableResources = createAsyncThunk('allocations/fetchBillableResources', async () =>
  allocationService.getBillableResources(),
);
export const fetchUtilization = createAsyncThunk('allocations/fetchUtilization', async () =>
  allocationService.getUtilization(),
);

const allocationSlice = createSlice({
  name: 'allocations',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchAllocations.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchAllocations.fulfilled, (state, action) => {
        state.loading = false;
        state.allocations = action.payload;
      })
      .addCase(fetchAllocations.rejected, (state) => {
        state.loading = false;
        state.error = 'Unable to load allocations.';
      })
      .addCase(createAllocation.fulfilled, (state, action) => {
        state.allocations.unshift(action.payload);
      })
      .addCase(updateAllocation.fulfilled, (state, action) => {
        state.allocations = state.allocations.map((allocation) =>
          allocation.id === action.payload.id ? action.payload : allocation,
        );
      })
      .addCase(deleteAllocation.fulfilled, (state, action) => {
        state.allocations = state.allocations.filter((allocation) => allocation.id !== action.payload);
      })
      .addCase(fetchBenchResources.fulfilled, (state, action) => {
        state.benchResources = action.payload;
      })
      .addCase(fetchBillableResources.fulfilled, (state, action) => {
        state.billableResources = action.payload;
      })
      .addCase(fetchUtilization.fulfilled, (state, action) => {
        state.utilization = action.payload;
      });
  },
});

export default allocationSlice.reducer;
