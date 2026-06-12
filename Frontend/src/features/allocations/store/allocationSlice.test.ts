import reducer, { fetchAllocations, createAllocation, updateAllocation, deleteAllocation } from './allocationSlice';
import type { AllocationState } from './allocationSlice';
import type { ResourceAllocationFormValues } from '../types/resourceAllocation';

const initialState: AllocationState = {
  allocations: [],
  benchResources: [],
  billableResources: [],
  utilization: [],
  loading: false,
  error: null,
};

describe('allocationSlice', () => {
  it('should return the initial state', () => {
    expect(reducer(undefined, { type: 'unknown' })).toEqual(initialState);
  });

  it('should handle fetchAllocations.pending', () => {
    const nextState = reducer(initialState, fetchAllocations.pending('', undefined));
    expect(nextState.loading).toBe(true);
    expect(nextState.error).toBeNull();
  });

  it('should handle fetchAllocations.fulfilled', () => {
    const payload = [
      {
        id: 1,
        employeeId: 1,
        projectId: 1,
        allocationPercentage: 80,
        allocationType: 1,
        startDate: '2026-06-02',
        endDate: '2026-07-02',
        isActive: true,
        createdDate: '2026-06-02',
        modifiedDate: '2026-06-02',
      },
    ];
    const nextState = reducer(initialState, fetchAllocations.fulfilled(payload, '', undefined));
    expect(nextState.loading).toBe(false);
    expect(nextState.allocations).toEqual(payload);
  });

  it('should handle createAllocation.fulfilled', () => {
    const payload = {
      id: 2,
      employeeId: 2,
      projectId: 1,
      allocationPercentage: 50,
      allocationType: 1,
      startDate: '2026-06-02',
      endDate: '2026-08-02',
      isActive: true,
      createdDate: '2026-06-02',
      modifiedDate: '2026-06-02',
    };
    const nextState = reducer(initialState, createAllocation.fulfilled(payload, '', {} as ResourceAllocationFormValues));
    expect(nextState.allocations[0]).toEqual(payload);
  });

  it('should handle updateAllocation.fulfilled', () => {
    const payload = {
      id: 1,
      employeeId: 1,
      projectId: 1,
      allocationPercentage: 90,
      allocationType: 1,
      startDate: '2026-06-02',
      endDate: '2026-07-02',
      isActive: true,
      createdDate: '2026-06-02',
      modifiedDate: '2026-06-02',
    };
    const beforeState: AllocationState = {
      ...initialState,
      allocations: [
        {
          ...payload,
          allocationPercentage: 80,
        },
      ],
    };

    const nextState = reducer(beforeState, updateAllocation.fulfilled(payload, '', { id: 1, values: {} as ResourceAllocationFormValues }));
    expect(nextState.allocations[0].allocationPercentage).toBe(90);
  });

  it('should handle deleteAllocation.fulfilled', () => {
    const beforeState: AllocationState = {
      ...initialState,
      allocations: [
        {
          id: 1,
          employeeId: 1,
          projectId: 1,
          allocationPercentage: 80,
          allocationType: 1,
          startDate: '2026-06-02',
          endDate: '2026-07-02',
          isActive: true,
          createdDate: '2026-06-02',
          modifiedDate: '2026-06-02',
        },
      ],
    };

    const nextState = reducer(beforeState, deleteAllocation.fulfilled(1, '', 1));
    expect(nextState.allocations).toHaveLength(0);
  });
});
