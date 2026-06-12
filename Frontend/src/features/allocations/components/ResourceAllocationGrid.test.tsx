import { render, screen } from '@testing-library/react';
import ResourceAllocationGrid from './ResourceAllocationGrid';
import type { ResourceAllocation } from '../types/resourceAllocation';

const sampleAllocations: ResourceAllocation[] = [
  {
    id: 1,
    employeeId: 1,
    projectId: 1,
    allocationPercentage: 50,
    allocationType: 1,
    startDate: '2026-06-02',
    endDate: '2026-07-02',
    isActive: true,
    createdDate: '2026-06-02',
    modifiedDate: '2026-06-02',
  },
];

const employees = [
  {
    id: 1,
    employeeCode: 'EMP-001',
    firstName: 'Jane',
    lastName: 'Doe',
    email: 'jane.doe@example.com',
    phone: '555-0101',
    department: 'Engineering',
    designation: 'Developer',
    experience: 5,
    joiningDate: '2023-01-01',
    status: 'Active' as const,
  },
];

const projects = [
  {
    id: 1,
    projectCode: 'PRJ-001',
    projectName: 'Apollo',
    description: 'Platform modernization',
    clientName: 'Acme Corp',
    clientContact: 'client@example.com',
    projectManager: 'Jack Lee',
    startDate: '2026-05-01',
    endDate: '2026-12-31',
    status: 'Active' as const,
    priority: 'High' as const,
    technologies: ['React', 'Azure'],
    allocatedResources: 5,
  },
];

describe('ResourceAllocationGrid', () => {
  it('renders grid headers and allocation rows', () => {
    render(
      <ResourceAllocationGrid
        allocations={sampleAllocations}
        employees={employees}
        projects={projects}
        loading={false}
        onEdit={() => undefined}
        onDelete={() => undefined}
      />, 
    );

    expect(screen.getByText(/Resource allocations/i)).toBeInTheDocument();
    expect(screen.getByText(/Employee/i)).toBeInTheDocument();
    expect(screen.getByText(/Project/i)).toBeInTheDocument();
  });
});
