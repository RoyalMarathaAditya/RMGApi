import { Box, Chip } from '@mui/material';
import { DataGrid } from '@mui/x-data-grid';
import type { GridColDef } from '@mui/x-data-grid';
import type { EmployeeSkill } from '../types';

export default function EmployeeSkillGrid({ employeeSkills }: { employeeSkills: EmployeeSkill[] }) {
  const columns: GridColDef<EmployeeSkill>[] = [
    { field: 'employeeName', headerName: 'Employee', minWidth: 180, flex: 1 },
    { field: 'skillName', headerName: 'Skill', minWidth: 160, flex: 1 },
    { field: 'department', headerName: 'Department', minWidth: 140, flex: 0.8 },
    { field: 'project', headerName: 'Project', minWidth: 130, flex: 0.8 },
    {
      field: 'proficiency',
      headerName: 'Proficiency',
      minWidth: 150,
      flex: 0.8,
      renderCell: (params) => <Chip label={params.row.proficiency} size="small" />,
    },
    { field: 'yearsOfExperience', headerName: 'Experience', minWidth: 120, flex: 0.6, type: 'number' },
    { field: 'certification', headerName: 'Certification', minWidth: 180, flex: 1 },
  ];

  return (
    <Box sx={{ minHeight: 420, width: '100%' }}>
      <DataGrid columns={columns} disableRowSelectionOnClick pageSizeOptions={[5, 10, 20]} rows={employeeSkills} sx={{ border: 0 }} />
    </Box>
  );
}
