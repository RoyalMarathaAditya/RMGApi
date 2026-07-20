
import { Box } from '@mui/material';
import { DataGrid } from '@mui/x-data-grid';
import type { GridColDef, GridPaginationModel } from '@mui/x-data-grid';
import type { Employee } from '../types/employee';

interface EmployeeGridProps {
  columns: GridColDef<Employee>[];
  loading: boolean;
  paginationModel: GridPaginationModel;
  rows: Employee[];
  onPaginationModelChange: (model: GridPaginationModel) => void;
}

export default function EmployeeGrid({
  columns,
  loading,
  paginationModel,
  rows,
  onPaginationModelChange,
}: EmployeeGridProps) {
  const visibleRows = Math.max(1, Math.min(rows.length, paginationModel.pageSize));
  const gridHeight = 112 + visibleRows * 52;

  return (
    <Box sx={{ height: gridHeight, maxHeight: 560, minHeight: 220, width: '100%', overflowX: 'auto' }}>
      <DataGrid
        columns={columns}
        loading={loading}
        onPaginationModelChange={onPaginationModelChange}
        pageSizeOptions={[5, 10, 25]}
        paginationModel={paginationModel}
        rows={rows}
        sx={{
          border: 0,
          '& .MuiDataGrid-columnHeaderTitle': {
            fontWeight: 700,
            whiteSpace: 'nowrap',
          },
          '& .MuiDataGrid-columnHeaders': {
            backgroundColor: 'grey.50',
          },
          '& .MuiDataGrid-cell': {
            whiteSpace: 'nowrap',
          },
        }}
      />
    </Box>
  );
}
