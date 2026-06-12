import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import DeleteOutlineOutlinedIcon from '@mui/icons-material/DeleteOutlineOutlined';
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid';
import type { GridColDef } from '@mui/x-data-grid';
import { Paper, Typography } from '@mui/material';
import type { Employee } from '../../employees/types/employee';
import type { Project } from '../../projects/types/project.types';
import type { ResourceAllocation } from '../types/resourceAllocation';
import { allocationTypeLabelMap } from '../types/allocationType';

interface ResourceAllocationGridProps {
  allocations: ResourceAllocation[];
  employees: Employee[];
  projects: Project[];
  loading: boolean;
  onEdit: (allocation: ResourceAllocation) => void;
  onDelete: (allocation: ResourceAllocation) => void;
}

function getEmployeeName(employeeId: number, employees: Employee[]) {
  const employee = employees.find((item) => item.id === employeeId);
  return employee ? `${employee.firstName} ${employee.lastName}` : 'Unknown';
}

function getProjectName(projectId: number, projects: Project[]) {
  const project = projects.find((item) => item.id === projectId);
  return project ? project.projectName : 'Unknown';
}

export default function ResourceAllocationGrid({
  allocations,
  employees,
  projects,
  loading,
  onEdit,
  onDelete,
}: ResourceAllocationGridProps) {
  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    {
      field: 'employeeName',
      headerName: 'Employee',
      flex: 1,
      valueGetter: (params) => getEmployeeName(params.row.employeeId, employees),
    },
    {
      field: 'projectName',
      headerName: 'Project',
      flex: 1,
      valueGetter: (params) => getProjectName(params.row.projectId, projects),
    },
    {
      field: 'allocationPercentage',
      headerName: 'Allocation %',
      type: 'number',
      width: 130,
      valueFormatter: ({ value }) => `${value}%`,
    },
    {
      field: 'allocationType',
      headerName: 'Type',
      width: 140,
      valueFormatter: ({ value }) => allocationTypeLabelMap[value as number] ?? 'Unknown',
    },
    {
      field: 'startDate',
      headerName: 'Start',
      width: 120,
    },
    {
      field: 'endDate',
      headerName: 'End',
      width: 120,
    },
    {
      field: 'isActive',
      headerName: 'Status',
      width: 110,
      valueFormatter: ({ value }) => (value ? 'Active' : 'Inactive'),
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 120,
      getActions: (params) => [
        <GridActionsCellItem
          icon={<EditOutlinedIcon />}
          label="Edit"
          onClick={() => onEdit(params.row as ResourceAllocation)}
          key="edit"
        />,
        <GridActionsCellItem
          icon={<DeleteOutlineOutlinedIcon />}
          label="Delete"
          onClick={() => onDelete(params.row as ResourceAllocation)}
          key="delete"
        />,
      ],
    },
  ];

  return (
    <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, height: 680, overflow: 'hidden' }}>
      <Typography sx={{ p: 2, pb: 0 }} variant="h6">
        Resource allocations
      </Typography>
      <DataGrid
        autoHeight
        checkboxSelection={false}
        columns={columns}
        density="comfortable"
        disableSelectionOnClick
        rows={allocations}
        loading={loading}
        pageSizeOptions={[10, 20, 50]}
        sx={{ border: 0, minHeight: 540 }}
      />
    </Paper>
  );
}
