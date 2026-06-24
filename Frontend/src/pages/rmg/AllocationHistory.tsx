import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import {
  Box,
  Chip,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material';
import PageContainer from '../../components/common/PageContainer';
import { allocationService } from '../../features/rmg/services/allocationService';
import type { AllocationHistoryDto } from '../../features/rmg/types/allocation';

const changeTypeColors: Record<string, 'success' | 'info' | 'warning' | 'error'> = {
  Created: 'success',
  Updated: 'info',
  Deleted: 'error',
};

export default function AllocationHistory() {
  const { allocationId } = useParams<{ allocationId: string }>();
  const [history, setHistory] = useState<AllocationHistoryDto[]>([]);

  useEffect(() => {
    if (allocationId) {
      loadHistory();
    }
  }, [allocationId]);

  const loadHistory = async () => {
    try {
      const data = await allocationService.getHistory(Number(allocationId));
      setHistory(data);
    } catch {
      console.error('Failed to load history');
    }
  };

  return (
    <PageContainer title="Allocation History">
      <TableContainer component={Paper} elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell sx={{ fontWeight: 700 }}>Date</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>Type</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>Old Project</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>New Project</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>Old %</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>New %</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>Old Status</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>New Status</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>Modified By</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>Remarks</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {history.map((h) => (
              <TableRow key={h.id} hover>
                <TableCell>{new Date(h.modifiedDate).toLocaleString()}</TableCell>
                <TableCell>
                  <Chip label={h.changeType} size="small" color={changeTypeColors[h.changeType] ?? 'default'} variant="outlined" />
                </TableCell>
                <TableCell>{h.oldProject ?? '-'}</TableCell>
                <TableCell>{h.newProject ?? '-'}</TableCell>
                <TableCell>{h.oldAllocationPercentage != null ? `${h.oldAllocationPercentage}%` : '-'}</TableCell>
                <TableCell>{h.newAllocationPercentage != null ? `${h.newAllocationPercentage}%` : '-'}</TableCell>
                <TableCell>{h.oldStatus ?? '-'}</TableCell>
                <TableCell>{h.newStatus ?? '-'}</TableCell>
                <TableCell>{h.modifiedBy}</TableCell>
                <TableCell>
                  <Typography variant="caption" color="text.secondary">{h.remarks ?? '-'}</Typography>
                </TableCell>
              </TableRow>
            ))}
            {history.length === 0 && (
              <TableRow>
                <TableCell colSpan={10} align="center" sx={{ py: 4 }}>No history found</TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>
    </PageContainer>
  );
}
