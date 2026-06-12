import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import { Box, Chip } from '@mui/material';
import type { GridColDef } from '@mui/x-data-grid';
import { GridActionsCellItem } from '@mui/x-data-grid';
import type { Skill } from '../../skills/types';
import type { Employee } from '../types/employee';

interface EmployeeColumnHandlers {
  skills: Skill[];
  onEdit: (employee: Employee) => void;
  onDelete: (employee: Employee) => void;
}

export function getEmployeeColumns({ skills, onEdit, onDelete }: EmployeeColumnHandlers): GridColDef<Employee>[] {
  const skillNameById = new Map(skills.map((skill) => [skill.id, skill.skillName]));

  return [
    { field: 'employeeCode', headerName: 'Code', minWidth: 120, flex: 0.7 },
    {
      field: 'name',
      headerName: 'Name',
      minWidth: 180,
      flex: 1,
      valueGetter: (_value, row) => `${row.firstName} ${row.lastName}`,
    },
    { field: 'email', headerName: 'Email', minWidth: 220, flex: 1.2 },
    { field: 'department', headerName: 'Department', minWidth: 150, flex: 0.9 },
    { field: 'designation', headerName: 'Designation', minWidth: 190, flex: 1 },
    { field: 'role', headerName: 'Role', minWidth: 130, flex: 0.7 },
    {
      field: 'skillIds',
      headerName: 'Skills',
      minWidth: 220,
      flex: 1.2,
      sortable: false,
      valueGetter: (_value, row) =>
        row.skillIds.map((skillId) => skillNameById.get(skillId)).filter(Boolean).join(', '),
      renderCell: ({ row }) => {
        const skillNames = row.skillIds
          .map((skillId) => skillNameById.get(skillId))
          .filter((skillName): skillName is string => Boolean(skillName));
        const visibleSkillNames = skillNames.slice(0, 2);

        return (
          <Box alignItems="center" display="flex" flexWrap="wrap" gap={0.5} sx={{ py: 0.5 }}>
            {visibleSkillNames.map((skillName) => (
              <Chip key={skillName} label={skillName} size="small" />
            ))}
            {skillNames.length > visibleSkillNames.length ? (
              <Chip label={`+${skillNames.length - visibleSkillNames.length}`} size="small" variant="outlined" />
            ) : null}
          </Box>
        );
      },
    },
    {
      field: 'experience',
      headerName: 'Exp.',
      align: 'right',
      headerAlign: 'right',
      minWidth: 90,
      type: 'number',
      valueFormatter: (value) => `${value} yrs`,
    },
    {
      field: 'status',
      headerName: 'Status',
      minWidth: 120,
      renderCell: ({ row }) => (
        <Chip
          color={row.status === 'Active' ? 'success' : row.status === 'On Leave' ? 'warning' : 'default'}
          label={row.status}
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
