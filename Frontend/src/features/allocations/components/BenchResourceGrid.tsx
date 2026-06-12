import { DataGrid } from '@mui/x-data-grid';
import type { GridColDef } from '@mui/x-data-grid';
import { Paper, Typography } from '@mui/material';
import type { BenchResource } from '../types/benchResource';

interface BenchResourceGridProps {
  benchResources: BenchResource[];
  loading: boolean;
}

export default function BenchResourceGrid({ benchResources, loading }: BenchResourceGridProps) {
  const columns: GridColDef[] = [
    { field: 'employeeId', headerName: 'Employee ID', width: 110 },
    { field: 'employeeName', headerName: 'Employee', flex: 1, minWidth: 180 },
    { field: 'department', headerName: 'Department', width: 160 },
    { field: 'benchDays', headerName: 'Bench Days', width: 120, type: 'number' },
    { field: 'lastProject', headerName: 'Last Project', flex: 1, minWidth: 180 },
    { field: 'availabilityDate', headerName: 'Available From', width: 140 },
  ];

  return (
    <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 2 }}>
      <Typography gutterBottom variant="h6">
        Bench resources
      </Typography>
      <DataGrid
        autoHeight
        columns={columns}
        density="comfortable"
        disableSelectionOnClick
        loading={loading}
        rows={benchResources}
        pageSizeOptions={[10, 20, 50]}
        sx={{ border: 0 }}
      />
    </Paper>
  );
}
