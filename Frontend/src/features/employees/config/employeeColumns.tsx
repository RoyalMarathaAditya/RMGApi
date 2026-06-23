
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import { Chip } from '@mui/material';
import type { GridColDef } from '@mui/x-data-grid';
import { GridActionsCellItem } from '@mui/x-data-grid';
import type { Employee } from '../types/employee';

interface EmployeeColumnHandlers {
  onEdit: (employee: Employee) => void;
  onDelete: (employee: Employee) => void;
}

export function getEmployeeColumns({ onEdit, onDelete }: EmployeeColumnHandlers): GridColDef<Employee>[] {
  return [
    { field: 'employeeCode', headerName: 'Code', minWidth: 100, flex: 0.6 },
    { field: 'fullName', headerName: 'Name', minWidth: 180, flex: 1.2 },
    { field: 'email', headerName: 'Email', minWidth: 200, flex: 1.2 },
    { field: 'designation', headerName: 'Designation', minWidth: 160, flex: 1 },
    { field: 'employmentType', headerName: 'Emp Type', minWidth: 120, flex: 0.8 },
    { field: 'practice', headerName: 'Practice', minWidth: 150, flex: 1 },
    { field: 'workModel', headerName: 'Work Model', minWidth: 120, flex: 0.8 },
    { field: 'location', headerName: 'Location', minWidth: 120, flex: 0.8 },
    {
      field: 'priorExperience',
      headerName: 'Prior Exp.',
      align: 'right',
      headerAlign: 'right',
      minWidth: 100,
      type: 'number',
      valueFormatter: (value) => value ? `${value} yrs` : '',
    },
    {
      field: 'employeeStatus',
      headerName: 'Status',
      minWidth: 110,
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
      field: 'actions',
      headerName: '',
      type: 'actions',
      width: 96,
      getActions: ({ row }) => [
        <GridActionsCellItem icon={<EditIcon />} key="edit" label="Edit" onClick={() => onEdit(row)} />,
        <GridActionsCellItem
          icon={<DeleteIcon />}
          key="delete"
          label="Delete"
          onClick={() => onDelete(row)}
          showInMenu
        />,
      ],
    },
  ];
}
