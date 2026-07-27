import api from '../../../services/api';
import type {
  PracticeWiseReportDto,
  ClientWiseReportDto,
  ProjectWiseReportDto,
  ReportChartDataDto,
} from '../types/report';

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

export const reportService = {
  async getPracticeWiseReport(engineeringOnly?: boolean): Promise<PracticeWiseReportDto[]> {
    const response = await api.get<PracticeWiseReportDto[]>('/reports/practice-wise', {
      params: engineeringOnly ? { engineeringOnly: true } : undefined,
    });
    return unwrap(response);
  },

  async exportPracticeWiseReport(engineeringOnly?: boolean) {
    const url = engineeringOnly
      ? '/reports/practice-wise/export?engineeringOnly=true'
      : '/reports/practice-wise/export';
    await exportBlob(url, 'PracticeWiseReport');
  },

  async getClientWiseReport(params?: {
    client?: string;
    practice?: string;
    status?: string;
  }): Promise<ClientWiseReportDto[]> {
    const response = await api.get<ClientWiseReportDto[]>('/reports/client-wise', { params });
    return unwrap(response);
  },

  async getClientWiseChartData(): Promise<ReportChartDataDto> {
    const response = await api.get<ReportChartDataDto>('/reports/client-wise/charts');
    return unwrap(response);
  },

  async getProjectWiseReport(params?: {
    client?: string;
    practice?: string;
    status?: string;
  }): Promise<ProjectWiseReportDto[]> {
    const response = await api.get<ProjectWiseReportDto[]>('/reports/project-wise', { params });
    return unwrap(response);
  },

  async getProjectWiseChartData(): Promise<ReportChartDataDto> {
    const response = await api.get<ReportChartDataDto>('/reports/project-wise/charts');
    return unwrap(response);
  },
};
