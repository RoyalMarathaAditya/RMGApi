import api from '../../../services/api';
import type { Employee, EmployeeFormValues } from '../types/employee';

export interface UploadColumnInfo {
  field: string;
  header: string;
}

export interface BulkUploadResponse {
  success: boolean;
  totalRows: number;
  successRows: number;
  failedRows: number;
  errors: Array<{ rowNumber: number; employeeName: string | null; email: string | null; errorMessage: string }>;
  errorFileUrl: string | null;
  columns?: UploadColumnInfo[] | null;
  importedRows?: Array<{
    rowNumber: number;
    employeeCode: string | null;
    fullName: string;
    employeeType: string;
    designation: string;
    practice: string;
    subPractice: string | null;
    nvLocation: string | null;
    reportingManager: string | null;
    practiceHead: string | null;
    email: string;
    activeStatus: string | null;
    doj: string | null;
    lwd: string | null;
  }> | null;
}

function unwrap<T>(response: { data: { success: boolean; data: T } }): T {
  return response.data.data;
}

function normalizeEmployee(data: any): Employee {
  return {
    id: data.id,
    employeeCode: data.employeeCode ?? '',
    fullName: data.fullName ?? '',
    email: data.email ?? '',
    doj: data.doj ? data.doj.slice(0, 10) : '',
    lwd: data.lwd ? data.lwd.slice(0, 10) : null,
    priorExperience: data.priorExperience ?? 0,
    relevantExperience: data.relevantExperience ?? null,
    mobileNumber: data.mobileNumber ?? null,
    deloitteFitment: data.deloitteFitment ?? null,
    engineering: data.engineering ?? null,
    employmentTypeId: data.employmentTypeId ?? '',
    employmentType: data.employmentType ?? '',
    locationId: data.locationId ?? '',
    location: data.location ?? '',
    workModelId: data.workModelId ?? '',
    workModel: data.workModel ?? '',
    practiceId: data.practiceId ?? '',
    practice: data.practice ?? '',
    departmentTypeId: data.departmentTypeId ?? '',
    departmentType: data.departmentType ?? '',
    statusId: data.statusId ?? '',
    employeeStatus: data.employeeStatus ?? '',
    reportingManagerName: data.reportingManagerName ?? null,
    practiceHeadName: data.practiceHeadName ?? null,
    designationId: data.designationId ?? null,
    designation: data.designation ?? null,
    subPracticeId: data.subPracticeId ?? null,
    subPractice: data.subPractice ?? null,
    skills: data.skills ?? [],
  };
}

export const employeeService = {
  async getAll() {
    const response = await api.get('/employees');
    const list: any[] = unwrap(response);
    return list.map(normalizeEmployee);
  },

  async getLastUploadColumns(): Promise<UploadColumnInfo[]> {
    const response = await api.get('/employees/last-upload-columns');
    return response.data as UploadColumnInfo[];
  },

  async create(values: EmployeeFormValues) {
    const response = await api.post('/employees', {
      employeeCode: values.employeeCode,
      fullName: values.fullName,
      email: values.email,
      doj: values.doj,
      lwd: values.lwd || null,
      priorExperience: values.priorExperience,
      relevantExperience: values.relevantExperience || null,
      employmentTypeId: values.employmentTypeId,
      locationId: values.locationId,
      workModelId: values.workModelId,
      practiceId: values.practiceId,
      subPracticeId: values.subPracticeId || null,
      departmentTypeId: values.departmentTypeId,
      statusId: values.statusId,
      reportingManagerName: values.reportingManagerName || null,
      practiceHeadName: values.practiceHeadName || null,
      designationId: values.designationId || null,
      deloitteFitment: values.deloitteFitment ?? null,
      engineering: values.engineering ?? null,
      mobileNumber: values.mobileNumber || null,
      skillIds: values.skillIds,
    });
    return normalizeEmployee(unwrap(response));
  },

  async update(id: number, values: EmployeeFormValues) {
    const response = await api.put(`/employees/${id}`, {
      employeeCode: values.employeeCode,
      fullName: values.fullName,
      email: values.email,
      doj: values.doj,
      lwd: values.lwd || null,
      priorExperience: values.priorExperience,
      relevantExperience: values.relevantExperience || null,
      employmentTypeId: values.employmentTypeId,
      locationId: values.locationId,
      workModelId: values.workModelId,
      practiceId: values.practiceId,
      subPracticeId: values.subPracticeId || null,
      departmentTypeId: values.departmentTypeId,
      statusId: values.statusId,
      reportingManagerName: values.reportingManagerName || null,
      practiceHeadName: values.practiceHeadName || null,
      designationId: values.designationId || null,
      deloitteFitment: values.deloitteFitment ?? null,
      engineering: values.engineering ?? null,
      mobileNumber: values.mobileNumber || null,
      skillIds: values.skillIds,
    });
    return normalizeEmployee(unwrap(response));
  },

  async delete(id: number) {
    await api.delete(`/employees/${id}`);
    return id;
  },

  async uploadEmployees(file: File, onProgress?: (percent: number) => void): Promise<BulkUploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    const response = await api.post('/employees/bulk-upload', formData, {
      timeout: 120000,
      headers: { 'Content-Type': undefined },
      onUploadProgress: (e) => {
        if (e.total && onProgress) {
          onProgress(Math.round((e.loaded / e.total) * 100));
        }
      },
    });
    return response.data as BulkUploadResponse;
  },

  downloadTemplate() {
    window.open(`${api.defaults.baseURL}/employees/download-template`, '_blank');
  },
};
