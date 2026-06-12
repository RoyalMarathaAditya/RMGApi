import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import VisibilityOutlinedIcon from '@mui/icons-material/VisibilityOutlined';
import { Box, IconButton, Stack, Tooltip } from '@mui/material';
import { DataGrid } from '@mui/x-data-grid';
import type { GridColDef } from '@mui/x-data-grid';
import ProjectStatusChip from './ProjectStatusChip';
import type { Project } from '../types/project.types';

interface ProjectTableProps {
  onDelete: (project: Project) => void;
  onEdit: (project: Project) => void;
  onView: (project: Project) => void;
  projects: Project[];
}

export default function ProjectTable({ onDelete, onEdit, onView, projects }: ProjectTableProps) {
  const columns: GridColDef<Project>[] = [
    { field: 'projectCode', headerName: 'Project Code', minWidth: 130, flex: 0.8 },
    { field: 'projectName', headerName: 'Project Name', minWidth: 180, flex: 1.2 },
    { field: 'clientName', headerName: 'Client Name', minWidth: 160, flex: 1 },
    {
      field: 'status',
      headerName: 'Status',
      minWidth: 130,
      flex: 0.8,
      renderCell: (params) => <ProjectStatusChip status={params.row.status} />,
    },
    { field: 'priority', headerName: 'Priority', minWidth: 110, flex: 0.7 },
    { field: 'startDate', headerName: 'Start Date', minWidth: 130, flex: 0.8 },
    { field: 'endDate', headerName: 'End Date', minWidth: 130, flex: 0.8 },
    {
      field: 'allocatedResources',
      headerName: 'Allocated Resources',
      minWidth: 160,
      flex: 0.8,
      type: 'number',
    },
    {
      field: 'actions',
      headerName: 'Actions',
      minWidth: 140,
      sortable: false,
      filterable: false,
      renderCell: (params) => (
        <Stack direction="row" spacing={0.5}>
          <Tooltip title="View project">
            <IconButton aria-label={`View ${params.row.projectName}`} onClick={() => onView(params.row)} size="small">
              <VisibilityOutlinedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Edit project">
            <IconButton aria-label={`Edit ${params.row.projectName}`} onClick={() => onEdit(params.row)} size="small">
              <EditOutlinedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Delete project">
            <IconButton
              aria-label={`Delete ${params.row.projectName}`}
              color="error"
              onClick={() => onDelete(params.row)}
              size="small"
            >
              <DeleteOutlinedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Stack>
      ),
    },
  ];

  return (
    <Box sx={{ minHeight: 520, width: '100%' }}>
      <DataGrid
        columns={columns}
        disableRowSelectionOnClick
        initialState={{
          pagination: {
            paginationModel: { page: 0, pageSize: 10 },
          },
          sorting: {
            sortModel: [{ field: 'projectCode', sort: 'asc' }],
          },
        }}
        pageSizeOptions={[5, 10, 20]}
        rows={projects}
        sx={{
          border: 0,
          '& .MuiDataGrid-columnHeaders': {
            bgcolor: 'background.default',
          },
        }}
      />
    </Box>
  );
}
