import api from '../../../services/api';
import type {
  AllocationDto,
  CreateAllocationDto,
  UpdateAllocationDto,
  AllocationHistoryDto,
  CalendarViewDto,
  TimelineViewDto,
} from '../types/allocation';

function unwrap<T>(response: { data: T }): T {
  return response.data;
}

export const allocationService = {
  async getAll(): Promise<AllocationDto[]> {
    const response = await api.get<AllocationDto[]>('/resource-allocations');
    return unwrap(response);
  },

  async getById(id: number): Promise<AllocationDto> {
    const response = await api.get<AllocationDto>(`/resource-allocations/${id}`);
    return unwrap(response);
  },

  async create(values: CreateAllocationDto): Promise<AllocationDto> {
    const response = await api.post<AllocationDto>('/resource-allocations', values);
    return unwrap(response);
  },

  async update(id: number, values: UpdateAllocationDto): Promise<AllocationDto> {
    const response = await api.put<AllocationDto>(`/resource-allocations/${id}`, values);
    return unwrap(response);
  },

  async delete(id: number): Promise<void> {
    await api.delete(`/resource-allocations/${id}`);
  },

  async getHistory(allocationId: number): Promise<AllocationHistoryDto[]> {
    const response = await api.get<AllocationHistoryDto[]>(`/resource-allocations/${allocationId}/history`);
    return unwrap(response);
  },

  async getCalendarData(): Promise<CalendarViewDto[]> {
    const response = await api.get<CalendarViewDto[]>('/resource-allocations/calendar');
    return unwrap(response);
  },

  async getTimelineData(): Promise<TimelineViewDto[]> {
    const response = await api.get<TimelineViewDto[]>('/resource-allocations/timeline');
    return unwrap(response);
  },
};
