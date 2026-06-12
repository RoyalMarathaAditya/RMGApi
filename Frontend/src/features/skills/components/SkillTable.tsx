import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import VisibilityOutlinedIcon from '@mui/icons-material/VisibilityOutlined';
import { Box, IconButton, Stack, Tooltip } from '@mui/material';
import { DataGrid } from '@mui/x-data-grid';
import type { GridColDef } from '@mui/x-data-grid';
import SkillCategoryChip from './SkillCategoryChip';
import SkillStatusChip from './SkillStatusChip';
import type { Skill } from '../types';

interface SkillTableProps {
  loading?: boolean;
  onDelete: (skill: Skill) => void;
  onEdit: (skill: Skill) => void;
  onView: (skill: Skill) => void;
  skills: Skill[];
}

export default function SkillTable({ loading = false, onDelete, onEdit, onView, skills }: SkillTableProps) {
  const columns: GridColDef<Skill>[] = [
    { field: 'skillCode', headerName: 'Skill Code', minWidth: 130, flex: 0.8 },
    { field: 'skillName', headerName: 'Skill Name', minWidth: 190, flex: 1.2 },
    {
      field: 'category',
      headerName: 'Category',
      minWidth: 140,
      flex: 0.9,
      renderCell: (params) => <SkillCategoryChip category={params.row.category} />,
    },
    {
      field: 'status',
      headerName: 'Status',
      minWidth: 130,
      flex: 0.8,
      renderCell: (params) => <SkillStatusChip status={params.row.status} />,
    },
    { field: 'employeeCount', headerName: 'Employee Count', minWidth: 150, flex: 0.8, type: 'number' },
    {
      field: 'actions',
      filterable: false,
      headerName: 'Actions',
      minWidth: 140,
      sortable: false,
      renderCell: (params) => (
        <Stack direction="row" spacing={0.5}>
          <Tooltip title="View skill">
            <IconButton aria-label={`View ${params.row.skillName}`} onClick={() => onView(params.row)} size="small">
              <VisibilityOutlinedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Edit skill">
            <IconButton aria-label={`Edit ${params.row.skillName}`} onClick={() => onEdit(params.row)} size="small">
              <EditOutlinedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Delete skill">
            <IconButton aria-label={`Delete ${params.row.skillName}`} color="error" onClick={() => onDelete(params.row)} size="small">
              <DeleteOutlinedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Stack>
      ),
    },
  ];

  return (
    <Box sx={{ minHeight: 540, width: '100%' }}>
      <DataGrid
        columns={columns}
        disableRowSelectionOnClick
        initialState={{
          pagination: { paginationModel: { page: 0, pageSize: 10 } },
          sorting: { sortModel: [{ field: 'skillCode', sort: 'asc' }] },
        }}
        loading={loading}
        pageSizeOptions={[5, 10, 20]}
        rows={skills}
        sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }}
      />
    </Box>
  );
}
