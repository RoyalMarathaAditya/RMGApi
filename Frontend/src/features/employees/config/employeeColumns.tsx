
import { Chip } from '@mui/material';
import type { GridColDef } from '@mui/x-data-grid';
import type { Employee } from '../types/employee';

export function getEmployeeColumns(): GridColDef<Employee>[] {
  return [
    { field: 'employeeCode', headerName: 'Emp Id', width: 100 },
    { field: 'fullName', headerName: 'Full Name', width: 180 },
    { field: 'employmentType', headerName: 'FTE/ Consultant', width: 130 },
    { field: 'designation', headerName: 'Role/Designation', width: 160 },
    { field: 'practice', headerName: 'OU 4 - Practice', width: 160 },
    { field: 'subPractice', headerName: 'OU 5 - Sub-practice', width: 150 },
    { field: 'location', headerName: 'Location', width: 130 },
    { field: 'reportingManagerName', headerName: 'L1 Manager', width: 160 },
    { field: 'practiceHeadName', headerName: 'Practice Head', width: 160 },
    { field: 'email', headerName: 'Email ID', width: 220 },
    {
      field: 'employeeStatus',
      headerName: 'Active',
      width: 100,
      renderCell: ({ row }) => (
        <Chip
          color={row.employeeStatus === 'Active' ? 'success' : 'default'}
          label={row.employeeStatus}
          size="small"
          variant="outlined"
        />
      ),
    },
    {
      field: 'doj',
      headerName: 'DOJ',
      width: 130,
      valueFormatter: (value) => value ? new Date(value).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' }) : '',
    },
    {
      field: 'lwd',
      headerName: 'LWD',
      width: 150,
      valueFormatter: (value) => value ? new Date(value).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' }) : '',
    },
  ];
}
