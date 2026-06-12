import { allocationData, benchResourceData, billableResourceData, utilizationData } from '../mock/allocationData';
import type { ResourceAllocation, ResourceAllocationFormValues } from '../types/resourceAllocation';

let allocations: ResourceAllocation[] = [...allocationData];
let nextId = Math.max(...allocations.map((allocation) => allocation.id)) + 1;

const wait = () => new Promise((resolve) => window.setTimeout(resolve, 250));

export const allocationService = {
  async getAll() {
    await wait();
    return [...allocations];
  },

  async getById(id: number) {
    await wait();
    return allocations.find((allocation) => allocation.id === id);
  },

  async create(values: ResourceAllocationFormValues) {
    await wait();
    const allocation: ResourceAllocation = {
      ...values,
      id: nextId,
      createdDate: new Date().toISOString().slice(0, 10),
      modifiedDate: new Date().toISOString().slice(0, 10),
    };

    nextId += 1;
    allocations = [allocation, ...allocations];
    return allocation;
  },

  async update(id: number, values: ResourceAllocationFormValues) {
    await wait();
    const allocation: ResourceAllocation = {
      ...values,
      id,
      createdDate: allocations.find((a) => a.id === id)?.createdDate ?? new Date().toISOString().slice(0, 10),
      modifiedDate: new Date().toISOString().slice(0, 10),
    };
    allocations = allocations.map((current) => (current.id === id ? allocation : current));
    return allocation;
  },

  async delete(id: number) {
    await wait();
    allocations = allocations.filter((allocation) => allocation.id !== id);
    return id;
  },

  async getBenchResources() {
    await wait();
    return [...benchResourceData];
  },

  async getBillableResources() {
    await wait();
    return [...billableResourceData];
  },

  async getUtilization() {
    await wait();
    return [...utilizationData];
  },
};
