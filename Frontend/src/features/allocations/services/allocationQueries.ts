import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { allocationService } from './allocationService';
import type { ResourceAllocationFormValues } from '../types/resourceAllocation';

export const useAllocationsQuery = () =>
  useQuery({
    queryKey: ['allocations'],
    queryFn: allocationService.getAll,
  });

export const useBenchResourcesQuery = () =>
  useQuery({
    queryKey: ['benchResources'],
    queryFn: allocationService.getBenchResources,
  });

export const useBillableResourcesQuery = () =>
  useQuery({
    queryKey: ['billableResources'],
    queryFn: allocationService.getBillableResources,
  });

export const useResourceUtilizationQuery = () =>
  useQuery({
    queryKey: ['utilization'],
    queryFn: allocationService.getUtilization,
  });

export const useAllocationMutations = () => {
  const queryClient = useQueryClient();

  const createAllocation = useMutation({
    mutationFn: allocationService.create,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['allocations'] }),
  });

  const updateAllocation = useMutation({
    mutationFn: ({ id, values }: { id: number; values: ResourceAllocationFormValues }) =>
      allocationService.update(id, values),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['allocations'] }),
  });

  const deleteAllocation = useMutation({
    mutationFn: allocationService.delete,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['allocations'] }),
  });

  return { createAllocation, updateAllocation, deleteAllocation };
};
