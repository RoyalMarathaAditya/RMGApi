import api from '../../../services/api';
import type { DashboardSummaryDto, DashboardGridDto, DashboardFilterDto, PaginatedResponse, GridQueryParams } from '../types/dashboard';
import type { ResourceAvailabilityDto, ResourceSuggestionDto, PracticeUtilizationDto } from '../types/allocation';

function unwrap<T>(response: { data: T }): T {
  return response.data;
}

async function exportBlob(url: string, fallbackPrefix: string) {
  const response = await api.get(url, { responseType: 'blob' });
  const blob = new Blob([response.data]);
  const link = document.createElement('a');
  link.href = window.URL.createObjectURL(blob);
  const disposition = response.headers['content-disposition'];
  const match = disposition?.match(/filename=(.+)/);
  link.download = match?.[1] ?? `${fallbackPrefix}_${new Date().toISOString().slice(0, 10)}.xlsx`;
  document.body.appendChild(link);
  link.click();
  link.remove();
  window.URL.revokeObjectURL(link.href);
}

function buildUrl(basePath: string, params: Record<string, string | number | undefined>): string {
  const queryParts: string[] = [];
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      queryParts.push(`${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`);
    }
  });
  return queryParts.length > 0 ? `${basePath}?${queryParts.join('&')}` : basePath;
}

export const dashboardService = {
  async getSummary(): Promise<DashboardSummaryDto> {
    const response = await api.get<DashboardSummaryDto>('/rmg-dashboard/summary');
    return unwrap(response);
  },

  async getGridData(filter?: DashboardFilterDto, paging?: GridQueryParams): Promise<PaginatedResponse<DashboardGridDto>> {
    const params: Record<string, string | number | undefined> = {};
    if (paging?.page) params.page = paging.page;
    if (paging?.pageSize) params.pageSize = paging.pageSize;
    if (paging?.sortField) params.sortField = paging.sortField;
    if (paging?.sortDirection) params.sortDirection = paging.sortDirection;
    if (filter) {
      if (filter.searchTerm) params.searchTerm = filter.searchTerm;
      if (filter.practice) params.practice = filter.practice;
      if (filter.resourceStatus) params.resourceStatus = filter.resourceStatus;
      if (filter.designation) params.designation = filter.designation;
      if (filter.department) params.department = filter.department;
    }
    const url = buildUrl('/rmg-dashboard/grid', params);
    const response = await api.get<PaginatedResponse<DashboardGridDto>>(url);
    return unwrap(response);
  },

  async exportGridData(filter?: DashboardFilterDto) {
    const params: Record<string, string | undefined> = {};
    if (filter?.searchTerm) params.searchTerm = filter.searchTerm;
    if (filter?.practice) params.practice = filter.practice;
    if (filter?.resourceStatus) params.resourceStatus = filter.resourceStatus;
    const url = buildUrl('/rmg-dashboard/export', params);
    await exportBlob(url, 'ResourceAllocation');
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
