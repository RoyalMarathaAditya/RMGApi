import { useEffect, useState } from 'react';
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
import type { AllocationDto } from '../types/allocation';
import { toastService } from '../../../services/toastService';

const statusColors: Record<string, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
  Active: 'success',
  Planned: 'info',
  Completed: 'default',
  Released: 'warning',
  Cancelled: 'error',
};

export default function ResourceAllocationList() {
  const navigate = useNavigate();
  const [allocations, setAllocations] = useState<AllocationDto[]>([]);
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(25);

  useEffect(() => {
    loadAllocations();
  }, []);

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
                <TableCell sx={{ fontWeight: 700 }}>Employee</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Project</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Start Date</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>End Date</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Allocation %</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Status</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Total Allocated</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Available</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {allocations.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage).map((a) => (
                <TableRow key={a.id} hover>
                  <TableCell>
                    <Typography fontWeight={600} variant="body2">{a.employeeName}</Typography>
                    <Typography variant="caption" color="text.secondary">{a.employeeCode}</Typography>
                  </TableCell>
                  <TableCell>{a.projectName}</TableCell>
                  <TableCell>{new Date(a.startDate).toLocaleDateString()}</TableCell>
                  <TableCell>{a.endDate ? new Date(a.endDate).toLocaleDateString() : '-'}</TableCell>
                  <TableCell>
                    <Typography fontWeight={600}>{a.allocationPercentage}%</Typography>
                  </TableCell>
                  <TableCell>
                    <Chip label={a.allocationStatus} size="small" color={statusColors[a.allocationStatus] ?? 'default'} variant="outlined" />
                  </TableCell>
                  <TableCell>{a.totalAllocated}%</TableCell>
                  <TableCell>{a.availableCapacity}%</TableCell>
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
                  <TableCell colSpan={9} align="center" sx={{ py: 4 }}>No allocations found</TableCell>
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
