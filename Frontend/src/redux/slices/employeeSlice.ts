// Redux slice for employee CRUD operations and search/filter state
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { employeeService } from '../../features/employees/services/employeeService';
import type { Employee, EmployeeFilters, EmployeeFormValues } from '../../features/employees/types/employee';

interface EmployeeState {
  employees: Employee[];
  filters: EmployeeFilters;
  loading: boolean;
  error: string | null;
}

const initialState: EmployeeState = {
  employees: [],
  filters: {
    search: '',
    status: 'All',
    practiceId: '',
    doj: '',
    statusId: '',
  },
  loading: false,
  error: null,
};

export const fetchEmployees = createAsyncThunk(
  'employees/fetchEmployees',
  async (params?: { fullName?: string; practiceId?: string; doj?: string; statusId?: string }) => {
    const queryParams: Record<string, string> = {};
    if (params?.fullName) queryParams.fullName = params.fullName;
    if (params?.practiceId) queryParams.practiceId = params.practiceId;
    if (params?.doj) queryParams.doj = params.doj;
    if (params?.statusId) queryParams.statusId = params.statusId;
    return employeeService.getAll(Object.keys(queryParams).length > 0 ? queryParams : undefined);
  },
);

export const createEmployee = createAsyncThunk(
  'employees/createEmployee',
  async (values: EmployeeFormValues) => employeeService.create(values),
);

export const updateEmployee = createAsyncThunk(
  'employees/updateEmployee',
  async ({ id, values }: { id: number; values: EmployeeFormValues }) =>
    employeeService.update(id, values),
);

export const deleteEmployee = createAsyncThunk('employees/deleteEmployee', async (id: number) =>
  employeeService.delete(id),
);

const employeeSlice = createSlice({
  name: 'employees',
  initialState,
  reducers: {
    setEmployeeSearch(state, action: { payload: string }) {
      state.filters.search = action.payload;
    },
    setEmployeeStatusFilter(state, action: { payload: EmployeeFilters['status'] }) {
      state.filters.status = action.payload;
    },
    setEmployeeFilter(state, action: { payload: Partial<EmployeeFilters> }) {
      Object.assign(state.filters, action.payload);
    },
    clearEmployeeFilters(state) {
      state.filters = { ...initialState.filters };
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchEmployees.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchEmployees.fulfilled, (state, action) => {
        state.loading = false;
        state.employees = action.payload;
      })
      .addCase(fetchEmployees.rejected, (state) => {
        state.loading = false;
        state.error = 'Unable to load employees.';
      })
      .addCase(createEmployee.fulfilled, (state, action) => {
        state.employees.unshift(action.payload);
      })
      .addCase(updateEmployee.fulfilled, (state, action) => {
        state.employees = state.employees.map((employee) =>
          employee.id === action.payload.id ? action.payload : employee,
        );
      })
      .addCase(deleteEmployee.fulfilled, (state, action) => {
        state.employees = state.employees.filter((employee) => employee.id !== action.payload);
      });
  },
});

export const { setEmployeeSearch, setEmployeeStatusFilter, setEmployeeFilter, clearEmployeeFilters } = employeeSlice.actions;
export default employeeSlice.reducer;
