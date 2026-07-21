import api from '../../../services/api';
import type { PracticeWiseReportDto } from '../types/report';

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
  async getPracticeWiseReport(): Promise<PracticeWiseReportDto[]> {
    const response = await api.get<PracticeWiseReportDto[]>('/reports/practice-wise');
    return unwrap(response);
  },

  async exportPracticeWiseReport() {
    await exportBlob('/reports/practice-wise/export', 'PracticeWiseReport');
  },
};
