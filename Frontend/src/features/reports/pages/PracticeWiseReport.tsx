import { useEffect, useState } from 'react';
import GroupOutlinedIcon from '@mui/icons-material/GroupOutlined';
import PeopleOutlinedIcon from '@mui/icons-material/PeopleOutlined';
import ShowChartOutlinedIcon from '@mui/icons-material/ShowChartOutlined';
import TrendingUpOutlinedIcon from '@mui/icons-material/TrendingUpOutlined';
import WorkOutlineOutlinedIcon from '@mui/icons-material/WorkOutlineOutlined';
import {
  Box,
  Button,
  FormControlLabel,
  Paper,
  Stack,
  Switch,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material';
import FileDownloadOutlinedIcon from '@mui/icons-material/FileDownloadOutlined';
import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid,
  Tooltip, ResponsiveContainer, Legend,
} from 'recharts';
import PageContainer from '../../../components/common/PageContainer';
import KpiCard from '../components/KpiCard';
import { reportService } from '../services/reportService';
import type { PracticeWiseReportDto } from '../types/report';

const EXPERIENCE_RANGE_LABELS = [
  'Less than 1 Year',
  '1-3 Years',
  '3-6 Years',
  '6-9 Years',
  '9-12 Years',
  'More than 12 Years',
];

function getRangeCount(practice: PracticeWiseReportDto, label: string): number {
  const found = practice.experienceRanges.find((r) => r.range === label);
  return found?.count ?? 0;
}

export default function PracticeWiseReport() {
  const [data, setData] = useState<PracticeWiseReportDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [engineeringOnly, setEngineeringOnly] = useState(true);

  const loadData = async () => {
    setLoading(true);
    try {
      const result = await reportService.getPracticeWiseReport(engineeringOnly);
      setData(result);
    } catch {
      console.error('Failed to load practice wise report');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [engineeringOnly]);

  const totals = {
    headcount: data.reduce((s, r) => s + r.totalHeadcount, 0),
    billable: data.reduce((s, r) => s + r.billableCount, 0),
    utilized: data.reduce((s, r) => s + r.utilizedCount, 0),
  };

  const overallBillability =
    totals.headcount > 0 ? Math.round((totals.billable / totals.headcount) * 100) : 0;
  const overallUtilization =
    totals.headcount > 0 ? Math.round((totals.utilized / totals.headcount) * 100) : 0;

  const chartData = data.map((p) => ({
    practiceName: p.practiceName,
    Billability: p.billabilityPercentage,
    Utilization: p.utilizationPercentage,
    EngineeringHeadcount: p.engineeringHeadcount,
  }));

  const colSpan = 8 + EXPERIENCE_RANGE_LABELS.length;

  return (
    <PageContainer title="Practice Wise Report">
      <Stack spacing={3}>
        <Stack direction="row" spacing={2} justifyContent="flex-end" alignItems="center">
          <FormControlLabel
            control={<Switch checked={engineeringOnly} onChange={(e) => setEngineeringOnly(e.target.checked)} />}
            label="Engineering Only"
          />
          <Button
            variant="outlined"
            startIcon={<FileDownloadOutlinedIcon />}
            onClick={() => reportService.exportPracticeWiseReport(engineeringOnly)}
          >
            Export
          </Button>
          <Button variant="contained" onClick={loadData}>
            Refresh
          </Button>
        </Stack>

        <Box
          sx={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
            gap: 3,
          }}
        >
          <KpiCard icon={GroupOutlinedIcon} label="Total Headcount" value={totals.headcount} color="primary.main" />
          <KpiCard icon={PeopleOutlinedIcon} label="Billable" value={totals.billable} color="success.main" />
          <KpiCard icon={WorkOutlineOutlinedIcon} label="Utilized" value={totals.utilized} color="info.main" />
          <KpiCard
            icon={TrendingUpOutlinedIcon}
            label="Billability %"
            value={`${overallBillability}%`}
            color={overallBillability >= 70 ? 'success.main' : overallBillability >= 40 ? 'warning.main' : 'error.main'}
          />
          <KpiCard
            icon={ShowChartOutlinedIcon}
            label="Utilization %"
            value={`${overallUtilization}%`}
            color={overallUtilization >= 70 ? 'success.main' : overallUtilization >= 40 ? 'warning.main' : 'error.main'}
          />
        </Box>

        {chartData.length > 0 && (
          <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 2 }}>
            <Typography variant="subtitle1" fontWeight={700} mb={2}>
              Billability, Utilization & Headcount Overview
            </Typography>
            <ResponsiveContainer width="100%" height={400}>
              <BarChart data={chartData} margin={{ top: 10, right: 30, left: 0, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="practiceName" />
                <YAxis yAxisId="left" domain={[0, 100]} unit="%" />
                <YAxis yAxisId="right" orientation="right" />
                <Tooltip formatter={(value: number, name: string) =>
                  name === 'Engineering Headcount' ? value : `${value}%`
                } />
                <Legend />
                <Bar yAxisId="left" dataKey="Billability" name="Billability" fill="#1976d2" radius={[4, 4, 0, 0]} />
                <Bar yAxisId="left" dataKey="Utilization" name="Utilization" fill="#2e7d32" radius={[4, 4, 0, 0]} />
                <Bar yAxisId="right" dataKey="EngineeringHeadcount" name="Engineering Headcount" fill="#ed6c02" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </Paper>
        )}

        <TableContainer
          component={Paper}
          elevation={0}
          sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflowX: 'auto' }}
        >
          <Table sx={{ minWidth: 1200 }}>
            <TableHead>
              <TableRow>
                <TableCell sx={{ fontWeight: 700 }}>Practice</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Total Headcount</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Billable</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Non-Billable</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Utilized</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Non-Utilized</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Billability %</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Utilization %</TableCell>
                {EXPERIENCE_RANGE_LABELS.map((label) => (
                  <TableCell key={label} sx={{ fontWeight: 700 }} align="center">
                    {label}
                  </TableCell>
                ))}
              </TableRow>
            </TableHead>
            <TableBody>
              {data.map((practice) => (
                <TableRow key={practice.practiceId} hover>
                  <TableCell>
                    <Typography fontWeight={600}>{practice.practiceName}</Typography>
                  </TableCell>
                  <TableCell align="center">{practice.totalHeadcount}</TableCell>
                  <TableCell align="center">{practice.billableCount}</TableCell>
                  <TableCell align="center">{practice.nonBillableCount}</TableCell>
                  <TableCell align="center">{practice.utilizedCount}</TableCell>
                  <TableCell align="center">{practice.nonUtilizedCount}</TableCell>
                  <TableCell align="center">
                    <Typography
                      fontWeight={700}
                      color={
                        practice.billabilityPercentage >= 70
                          ? 'success.main'
                          : practice.billabilityPercentage >= 40
                            ? 'warning.main'
                            : 'error.main'
                      }
                    >
                      {practice.billabilityPercentage}%
                    </Typography>
                  </TableCell>
                  <TableCell align="center">
                    <Typography
                      fontWeight={700}
                      color={
                        practice.utilizationPercentage >= 70
                          ? 'success.main'
                          : practice.utilizationPercentage >= 40
                            ? 'warning.main'
                            : 'error.main'
                      }
                    >
                      {practice.utilizationPercentage}%
                    </Typography>
                  </TableCell>
                  {EXPERIENCE_RANGE_LABELS.map((label) => (
                    <TableCell key={label} align="center">
                      {getRangeCount(practice, label)}
                    </TableCell>
                  ))}
                </TableRow>
              ))}
              {!loading && data.length === 0 && (
                <TableRow>
                  <TableCell colSpan={colSpan} align="center" sx={{ py: 4 }}>
                    No practice data available
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Stack>
    </PageContainer>
  );
}
