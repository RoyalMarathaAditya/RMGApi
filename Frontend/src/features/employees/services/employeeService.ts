
import api from '../../../services/api';
import type { Employee, EmployeeFormValues } from '../types/employee';

function normalizeEmployee(data: any): Employee {
  return {
    id: data.id,
    employeeCode: data.employeeCode ?? `EMP-${String(data.id).padStart(4, '0')}`,
    firstName: data.firstName ?? '',
    lastName: data.lastName ?? '',
    email: data.email ?? '',
    phone: data.phone ?? '',
    department: data.department ?? '',
    designation: data.designation ?? '',
    role: data.role ?? 'Employee',
    skillIds: data.skillIds ?? [],
    experience: data.experience ?? 0,
    joiningDate: data.dateOfJoining?.slice(0, 10) ?? new Date().toISOString().slice(0, 10),
    status: data.status ?? 'Active',
  };
}

export const employeeService = {
  async getAll() {
    const response = await api.get<Employee[]>('/employees');
    return response.data.map(normalizeEmployee);
  },

  async create(values: EmployeeFormValues) {
    const response = await api.post<Employee>('/employees', values);
    return normalizeEmployee(response.data);
  },

  async update(id: number, values: EmployeeFormValues) {
    const response = await api.put<Employee>(`/employees/${id}`, values);
    return normalizeEmployee(response.data);
  },

  async delete(id: number) {
    await api.delete(`/employees/${id}`);
    return id;
  },
};
