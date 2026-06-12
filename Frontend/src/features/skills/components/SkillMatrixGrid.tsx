import { Box, Chip } from '@mui/material';
import { DataGrid } from '@mui/x-data-grid';
import type { GridColDef } from '@mui/x-data-grid';
import type { Skill, SkillMatrixEmployee } from '../types';

interface SkillMatrixGridProps {
  rows: SkillMatrixEmployee[];
  skills: Skill[];
}

export default function SkillMatrixGrid({ rows, skills }: SkillMatrixGridProps) {
  const columns: GridColDef<SkillMatrixEmployee>[] = [
    { field: 'employeeName', headerName: 'Employee', minWidth: 180, flex: 1 },
    { field: 'department', headerName: 'Department', minWidth: 140, flex: 0.8 },
    { field: 'project', headerName: 'Project', minWidth: 140, flex: 0.8 },
    ...skills.slice(0, 12).map<GridColDef<SkillMatrixEmployee>>((skill) => ({
      field: `skill_${skill.id}`,
      headerName: skill.skillName,
      minWidth: 150,
      sortable: false,
      valueGetter: (_value, row) => row.skills[skill.id] ?? '',
      renderCell: (params) =>
        params.value ? <Chip color={params.value === 'Expert' ? 'success' : 'default'} label={params.value} size="small" /> : null,
    })),
  ];

  return (
    <Box sx={{ minHeight: 520, width: '100%' }}>
      <DataGrid columns={columns} disableRowSelectionOnClick getRowId={(row) => row.employeeId} rows={rows} sx={{ border: 0 }} />
    </Box>
  );
}
