import { DataGrid } from '@mui/x-data-grid';
import type { GridColDef } from '@mui/x-data-grid';
import { Paper, Typography } from '@mui/material';
import type { BillableResource } from '../types/billableResource';

interface BillableResourceGridProps {
  billableResources: BillableResource[];
  loading: boolean;
}

export default function BillableResourceGrid({ billableResources, loading }: BillableResourceGridProps) {
  const columns: GridColDef[] = [
    { field: 'employeeId', headerName: 'Employee ID', width: 110 },
    { field: 'employeeName', headerName: 'Employee', flex: 1, minWidth: 180 },
    { field: 'projectName', headerName: 'Project', flex: 1, minWidth: 180 },
    {
      field: 'allocationPercentage',
      headerName: 'Allocation %',
      width: 120,
      valueFormatter: ({ value }) => `${value}%`,
    },
    { field: 'startDate', headerName: 'Start Date', width: 140 },
    { field: 'endDate', headerName: 'End Date', width: 140 },
  ];

  return (
    <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 2 }}>
      <Typography gutterBottom variant="h6">
        Billable resources
      </Typography>
      <DataGrid
        autoHeight
        columns={columns}
        density="comfortable"
        disableSelectionOnClick
        loading={loading}
        rows={billableResources}
        pageSizeOptions={[10, 20, 50]}
        sx={{ border: 0 }}
      />
    </Paper>
  );
}
