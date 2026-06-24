import { useEffect, useState } from 'react';
import {
  Box,
  LinearProgress,
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
import PageContainer from '../../../components/common/PageContainer';
import { dashboardService } from '../services/dashboardService';
import type { PracticeUtilizationDto } from '../types/allocation';

export default function PracticeUtilization() {
  const [data, setData] = useState<PracticeUtilizationDto[]>([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const result = await dashboardService.getPracticeUtilization();
      setData(result);
    } catch {
      console.error('Failed to load practice utilization');
    }
  };

  return (
    <PageContainer title="Practice Utilization">
      <TableContainer component={Paper} elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell sx={{ fontWeight: 700 }}>Practice</TableCell>
              <TableCell sx={{ fontWeight: 700 }} align="center">Total Resources</TableCell>
              <TableCell sx={{ fontWeight: 700 }} align="center">Allocated</TableCell>
              <TableCell sx={{ fontWeight: 700 }} align="center">Available</TableCell>
              <TableCell sx={{ fontWeight: 700 }} align="center">Bench</TableCell>
              <TableCell sx={{ fontWeight: 700 }} align="center">Utilization %</TableCell>
              <TableCell sx={{ fontWeight: 700 }}>Utilization Bar</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {data.map((row) => (
              <TableRow key={row.practiceId} hover>
                <TableCell>
                  <Typography fontWeight={600}>{row.practiceName}</Typography>
                </TableCell>
                <TableCell align="center">{row.totalResources}</TableCell>
                <TableCell align="center">{row.allocatedResources}</TableCell>
                <TableCell align="center">{row.availableResources}</TableCell>
                <TableCell align="center">{row.benchResources}</TableCell>
                <TableCell align="center">
                  <Typography fontWeight={700} color={row.utilizationPercentage >= 70 ? 'success.main' : row.utilizationPercentage >= 40 ? 'warning.main' : 'error.main'}>
                    {row.utilizationPercentage}%
                  </Typography>
                </TableCell>
                <TableCell>
                  <Stack direction="row" spacing={1} alignItems="center">
                    <Box sx={{ flex: 1, maxWidth: 200 }}>
                      <LinearProgress
                        variant="determinate"
                        value={row.utilizationPercentage}
                        sx={{ height: 10, borderRadius: 5 }}
                        color={row.utilizationPercentage >= 70 ? 'success' : row.utilizationPercentage >= 40 ? 'warning' : 'error'}
                      />
                    </Box>
                  </Stack>
                </TableCell>
              </TableRow>
            ))}
            {data.length === 0 && (
              <TableRow>
                <TableCell colSpan={7} align="center" sx={{ py: 4 }}>No practice data available</TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>
    </PageContainer>
  );
}
