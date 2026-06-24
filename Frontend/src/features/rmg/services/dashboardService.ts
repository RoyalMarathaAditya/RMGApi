import api from '../../../services/api';
import type { DashboardSummaryDto, DashboardGridDto, DashboardFilterDto } from '../types/dashboard';
import type { ResourceAvailabilityDto, ResourceSuggestionDto, PracticeUtilizationDto } from '../types/allocation';

function unwrap<T>(response: { data: T }): T {
  return response.data;
}

export const dashboardService = {
  async getSummary(): Promise<DashboardSummaryDto> {
    const response = await api.get<DashboardSummaryDto>('/rmg-dashboard/summary');
    return unwrap(response);
  },

  async getGridData(filter?: DashboardFilterDto): Promise<DashboardGridDto[]> {
    const params = new URLSearchParams();
    if (filter) {
      Object.entries(filter).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          params.append(key, String(value));
        }
      });
    }
    const query = params.toString();
    const url = query ? `/rmg-dashboard/grid?${query}` : '/rmg-dashboard/grid';
    const response = await api.get<DashboardGridDto[]>(url);
    return unwrap(response);
  },

  async getSuitableResources(projectId: number): Promise<ResourceSuggestionDto[]> {
    const response = await api.get<ResourceSuggestionDto[]>(`/rmg-dashboard/suitable-resources/${projectId}`);
    return unwrap(response);
  },

  async getResourceAvailability(): Promise<ResourceAvailabilityDto[]> {
    const response = await api.get<ResourceAvailabilityDto[]>('/rmg-dashboard/resource-availability');
    return unwrap(response);
  },

  async getPracticeUtilization(): Promise<PracticeUtilizationDto[]> {
    const response = await api.get<PracticeUtilizationDto[]>('/rmg-dashboard/practice-utilization');
    return unwrap(response);
  },
};
