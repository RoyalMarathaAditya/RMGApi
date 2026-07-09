import { useEffect, useMemo, useState } from 'react';
import {
  Box,
  Button,
  Chip,
  IconButton,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  Tooltip,
  Typography,
} from '@mui/material';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined';
import AddOutlinedIcon from '@mui/icons-material/AddOutlined';
import { useNavigate } from 'react-router-dom';
import PageContainer from '../../../components/common/PageContainer';
import { allocationService } from '../services/allocationService';
import { columnMappingService } from '../../columnMappings/services/columnMappingService';
import type { AllocationDto } from '../types/allocation';
import type { ColumnMapping } from '../../columnMappings/types/columnMapping';
import { toastService } from '../../../services/toastService';

const statusColors: Record<string, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
  Active: 'success',
  Planned: 'info',
  Completed: 'default',
  Released: 'warning',
  Cancelled: 'error',
  Billable: 'success',
  'Non-Billable': 'warning',
  Shadow: 'info',
  'Full Time': 'info',
  'Part Time': 'warning',
  Contract: 'default',
  Sow: 'secondary',
  Available: 'success',
  'Partially Allocated': 'warning',
  'Fully Allocated': 'info',
  Overallocated: 'error',
  Bench: 'default',
  'On Leave': 'warning',
};

interface ColumnDef {
  field: keyof AllocationDto;
  headerName: string;
  dataType: string;
}

type CellRenderer = (row: AllocationDto) => React.ReactNode;

const cellRenderers: Partial<Record<keyof AllocationDto, CellRenderer>> = {
  employeeName: (row) => (
    <>
      <Typography fontWeight={600} variant="body2">{row.employeeName}</Typography>
      <Typography variant="caption" color="text.secondary">{row.employeeCode}</Typography>
    </>
  ),
  startDate: (row) => <>{new Date(row.startDate).toLocaleDateString()}</>,
  endDate: (row) => <>{row.endDate ? new Date(row.endDate).toLocaleDateString() : '-'}</>,
  allocationPercentage: (row) => <Typography fontWeight={600}>{row.allocationPercentage}%</Typography>,
  allocationStatus: (row) => (
    <Chip label={row.allocationStatus} size="small" color={statusColors[row.allocationStatus] ?? 'default'} variant="outlined" />
  ),
  totalAllocated: (row) => <>{row.totalAllocated}%</>,
  availableCapacity: (row) => <>{row.availableCapacity}%</>,
  employeeCode: (row) => <Typography variant="body2">{row.employeeCode}</Typography>,
  practice: (row) => <Chip label={row.practice} size="small" variant="outlined" />,
  designation: (row) => <Typography variant="body2">{row.designation ?? '-'}</Typography>,
  practiceHead: (row) => <Typography variant="body2">{row.practiceHead ?? '-'}</Typography>,
  allocationType: (row) => (
    <Chip label={row.allocationType ?? '-'} size="small" variant="outlined" color={statusColors[row.allocationType ?? ''] ?? 'default'} />
  ),
  billableStatus: (row) => (
    <Chip label={row.billableStatus ?? '-'} size="small" variant="outlined" color={statusColors[row.billableStatus ?? ''] ?? 'default'} />
  ),
  resourceStatus: (row) => (
    <Chip label={row.resourceStatus} size="small" variant="outlined" color={statusColors[row.resourceStatus] ?? 'default'} />
  ),
};

function defaultCellRenderer(row: AllocationDto, field: keyof AllocationDto): string {
  const val = row[field];
  if (val === null || val === undefined) return '-';
  return String(val);
}

const defaultColumns: ColumnDef[] = [
  { field: 'employeeName', headerName: 'Employee', dataType: 'string' },
  { field: 'projectName', headerName: 'Project', dataType: 'string' },
  { field: 'startDate', headerName: 'Start Date', dataType: 'datetime' },
  { field: 'endDate', headerName: 'End Date', dataType: 'datetime' },
  { field: 'allocationPercentage', headerName: 'Allocation %', dataType: 'decimal' },
  { field: 'allocationStatus', headerName: 'Status', dataType: 'string' },
  { field: 'totalAllocated', headerName: 'Total Allocated', dataType: 'decimal' },
  { field: 'availableCapacity', headerName: 'Available', dataType: 'decimal' },
];

export default function ResourceAllocationList() {
  const navigate = useNavigate();
  const [allocations, setAllocations] = useState<AllocationDto[]>([]);
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(25);
  const [columns, setColumns] = useState<ColumnDef[]>(defaultColumns);

  useEffect(() => {
    loadAllocations();
    loadColumns();
  }, []);

  const loadColumns = async () => {
    try {
      const data = await columnMappingService.getAll();
      const active = data
        .filter((m: ColumnMapping) => m.entityType === 'resource-allocation' && m.isActive)
        .sort((a: ColumnMapping, b: ColumnMapping) => a.displayOrder - b.displayOrder);
      if (active.length > 0) {
        console.log(`[RMG] Loaded ${active.length} dynamic columns from ${data.length} total mappings`);
        setColumns(active.map((m: ColumnMapping) => ({
          field: m.targetProperty.charAt(0).toLowerCase() + m.targetProperty.slice(1) as keyof AllocationDto,
          headerName: m.targetDisplayName,
          dataType: m.dataType,
        })));
      } else {
        console.warn(`[RMG] No resource-allocation mappings found in ${data.length} total mappings, using defaults`);
      }
    } catch (err) {
      console.error('[RMG] Failed to load column mappings, using defaults:', err);
    }
  };

  const loadAllocations = async () => {
    try {
      const data = await allocationService.getAll();
      setAllocations(data);
    } catch {
      console.error('Failed to load allocations');
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this allocation?')) return;
    try {
      await allocationService.delete(id);
      setAllocations((prev) => prev.filter((a) => a.id !== id));
      toastService.success('Allocation deleted successfully.');
    } catch {
      console.error('Failed to delete allocation');
      toastService.error('Failed to delete allocation');
    }
  };

  const colSpan = columns.length + 1;

  return (
    <PageContainer title="All Allocations">
      <Stack spacing={2}>
        <Stack direction="row" justifyContent="flex-end">
          <Button variant="contained" startIcon={<AddOutlinedIcon />} onClick={() => navigate('/rmg/create')}>
            New Allocation
          </Button>
        </Stack>

        <TableContainer component={Box} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
          <Table size="small">
            <TableHead>
              <TableRow>
                {columns.map((col) => (
                  <TableCell key={col.field} sx={{ fontWeight: 700 }}>{col.headerName}</TableCell>
                ))}
                <TableCell sx={{ fontWeight: 700 }} align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {allocations.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((a) => (
                <TableRow key={a.id} hover>
                  {columns.map((col) => (
                    <TableCell key={col.field}>
                      {cellRenderers[col.field]?.(a) ?? defaultCellRenderer(a, col.field)}
                    </TableCell>
                  ))}
                  <TableCell align="right">
                    <Tooltip title="View / Edit">
                      <IconButton size="small" onClick={() => navigate(`/rmg/view/${a.employeeId}`)}>
                        <EditOutlinedIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Delete">
                      <IconButton size="small" onClick={() => handleDelete(a.id)}>
                        <DeleteOutlinedIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  </TableCell>
                </TableRow>
              ))}
              {allocations.length === 0 && (
                <TableRow>
                  <TableCell colSpan={colSpan} align="center" sx={{ py: 4 }}>No allocations found</TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
          <TablePagination
            component="div"
            count={allocations.length}
            page={page}
            onPageChange={(_, p) => setPage(p)}
            rowsPerPage={rowsPerPage}
            onRowsPerPageChange={(e) => { setRowsPerPage(Number(e.target.value)); setPage(0); }}
            rowsPerPageOptions={[10, 25, 50]}
          />
        </TableContainer>
      </Stack>
    </PageContainer>
  );
}
