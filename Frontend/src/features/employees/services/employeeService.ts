
import api from '../../../services/api';
import type { Employee, EmployeeFormValues } from '../types/employee';

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
    reportingManagerId: data.reportingManagerId ?? null,
    reportingManagerName: data.reportingManagerName ?? null,
    practiceHeadId: data.practiceHeadId ?? null,
    practiceHeadName: data.practiceHeadName ?? null,
    designationId: data.designationId ?? null,
    designation: data.designation ?? null,
    skills: data.skills ?? [],
  };
}

export const employeeService = {
  async getAll() {
    const response = await api.get('/employees');
    const list: any[] = unwrap(response);
    return list.map(normalizeEmployee);
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
      departmentTypeId: values.departmentTypeId,
      statusId: values.statusId,
      reportingManagerId: values.reportingManagerId || null,
      practiceHeadId: values.practiceHeadId || null,
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
      departmentTypeId: values.departmentTypeId,
      statusId: values.statusId,
      reportingManagerId: values.reportingManagerId || null,
      practiceHeadId: values.practiceHeadId || null,
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

  async uploadEmployees(file: File, onProgress?: (percent: number) => void) {
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
    return response.data as {
      success: boolean;
      totalRows: number;
      successRows: number;
      failedRows: number;
      errors: Array<{ rowNumber: number; employeeName: string | null; email: string | null; errorMessage: string }>;
      errorFileUrl: string | null;
    };
  },

  downloadTemplate() {
    window.open(`${api.defaults.baseURL}/employees/download-template`, '_blank');
  },
};
