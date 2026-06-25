import { useEffect, useMemo, useState, SyntheticEvent } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Avatar,
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  CircularProgress,
  Divider,
  LinearProgress,
  Stack,
  Tab,
  Tabs,
  Typography,
  Alert,
  Grid,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  TablePagination,
  TableSortLabel,
} from '@mui/material';
import ArrowBackOutlinedIcon from '@mui/icons-material/ArrowBackOutlined';
import PersonOutlineOutlinedIcon from '@mui/icons-material/PersonOutlineOutlined';
import WorkspacePremiumOutlinedIcon from '@mui/icons-material/WorkspacePremiumOutlined';
import AssignmentOutlinedIcon from '@mui/icons-material/AssignmentOutlined';
import TimelineOutlinedIcon from '@mui/icons-material/TimelineOutlined';
import CalendarTodayOutlinedIcon from '@mui/icons-material/CalendarTodayOutlined';
import HistoryEduOutlinedIcon from '@mui/icons-material/HistoryEduOutlined';
import AccountBalanceOutlinedIcon from '@mui/icons-material/AccountBalanceOutlined';
import PageContainer from '../../../components/common/PageContainer';
import { allocationService } from '../services/allocationService';
import type { EmployeeResourceDetailsDto, ProjectAllocationDetailDto } from '../types/allocation';

function formatDate(dateStr: string | null | undefined): string {
  if (!dateStr) return '-';
  const d = new Date(dateStr);
  return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
}

function DetailRow({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        py: 1.25,
        borderBottom: '1px solid #e5e7eb',
        '&:last-child': { borderBottom: 'none' },
      }}
    >
      <Typography sx={{ fontSize: 13, fontWeight: 500, color: '#6B7280' }}>
        {label}
      </Typography>
      <Typography sx={{ fontSize: 15, fontWeight: 600, color: '#111827', textAlign: 'right', ml: 2 }}>
        {value ?? '-'}
      </Typography>
    </Box>
  );
}

function SectionHeader({ icon, title }: { icon: React.ReactNode; title: string }) {
  return (
    <Stack direction="row" alignItems="center" spacing={1} mb={2.5}>
      <Avatar sx={{ width: 28, height: 28, bgcolor: 'primary.50', color: 'primary.main', fontSize: '0.9rem' }}>
        {icon}
      </Avatar>
      <Typography variant="subtitle1" sx={{ fontWeight: 700, color: '#111827' }}>
        {title}
      </Typography>
    </Stack>
  );
}

const projectStatusColors: Record<string, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
  Billable: 'success',
  'Non-Billable': 'warning',
  Shadow: 'info',
  Active: 'success',
  Planned: 'info',
  Completed: 'default',
  Released: 'warning',
  Cancelled: 'error',
};

type SortKey = 'project' | 'allocationPercentage' | 'startDate' | 'endDate' | 'client';
type SortDir = 'asc' | 'desc';

export default function ResourceAllocationView() {
  const { employeeId } = useParams<{ employeeId: string }>();
  const navigate = useNavigate();
  const [data, setData] = useState<EmployeeResourceDetailsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [tabValue, setTabValue] = useState(0);
  const [allocationPage, setAllocationPage] = useState(0);
  const [allocationRowsPerPage, setAllocationRowsPerPage] = useState(5);
  const [sortKey, setSortKey] = useState<SortKey>('project');
  const [sortDir, setSortDir] = useState<SortDir>('asc');

  const id = Number(employeeId);

  useEffect(() => {
    if (employeeId) loadData();
  }, [employeeId]);

  const loadData = async () => {
    setLoading(true);
    setError('');
    try {
      const result = await allocationService.getEmployeeDetails(id);
      setData(result);
    } catch {
      setError('Failed to load employee details');
    } finally {
      setLoading(false);
    }
  };

  const handleTabChange = (_: SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  const totalAllocated = useMemo(() => {
    if (!data) return 0;
    return data.projectAllocations.reduce((sum, pa) => sum + (pa.allocationPercentage ?? 0), 0);
  }, [data]);

  const availableCapacity = Math.max(0, 100 - totalAllocated);

  const handleSort = (key: SortKey) => {
    if (sortKey === key) {
      setSortDir((prev) => (prev === 'asc' ? 'desc' : 'asc'));
    } else {
      setSortKey(key);
      setSortDir('asc');
    }
  };

  const sortedAllocations = useMemo(() => {
    if (!data) return [];
    const list = [...data.projectAllocations];
    list.sort((a, b) => {
      let cmp = 0;
      switch (sortKey) {
        case 'project':
          cmp = (a.project ?? '').localeCompare(b.project ?? '');
          break;
        case 'allocationPercentage':
          cmp = (a.allocationPercentage ?? 0) - (b.allocationPercentage ?? 0);
          break;
        case 'startDate':
          cmp = (a.startDate ?? '').localeCompare(b.startDate ?? '');
          break;
        case 'endDate':
          cmp = (a.endDate ?? '').localeCompare(b.endDate ?? '');
          break;
        case 'client':
          cmp = (a.client ?? '').localeCompare(b.client ?? '');
          break;
      }
      return sortDir === 'asc' ? cmp : -cmp;
    });
    return list;
  }, [data, sortKey, sortDir]);

  const paginatedAllocations = useMemo(() => {
    return sortedAllocations.slice(
      allocationPage * allocationRowsPerPage,
      allocationPage * allocationRowsPerPage + allocationRowsPerPage
    );
  }, [sortedAllocations, allocationPage, allocationRowsPerPage]);

  if (loading) {
    return (
      <PageContainer title="Employee Details">
        <Box display="flex" flexDirection="column" alignItems="center" justifyContent="center" py={12}>
          <CircularProgress size={48} thickness={4} />
          <Typography variant="body2" color="text.secondary" mt={2}>Loading employee details...</Typography>
        </Box>
      </PageContainer>
    );
  }

  if (error) {
    return (
      <PageContainer title="Employee Details">
        <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>
        <Button startIcon={<ArrowBackOutlinedIcon />} onClick={() => navigate('/rmg')}>Back to Dashboard</Button>
      </PageContainer>
    );
  }

  if (!data) {
    return (
      <PageContainer title="Employee Details">
        <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 6, textAlign: 'center' }}>
          <Typography variant="h6" color="text.secondary" gutterBottom>Employee Not Found</Typography>
          <Typography variant="body2" color="text.secondary" mb={3}>The requested employee does not exist or has been removed.</Typography>
          <Button startIcon={<ArrowBackOutlinedIcon />} onClick={() => navigate('/rmg')}>Back to Dashboard</Button>
        </Card>
      </PageContainer>
    );
  }

  return (
    <PageContainer title="Employee Details">
      <Stack spacing={3}>
        <Stack direction="row" alignItems="center" justifyContent="space-between" flexWrap="wrap" gap={2}>
          <Button startIcon={<ArrowBackOutlinedIcon />} onClick={() => navigate('/rmg')} size="small">Back to Dashboard</Button>
        </Stack>

        <Card
          elevation={0}
          sx={{
            borderRadius: 3,
            border: '1px solid',
            borderColor: 'divider',
            background: (theme) => `linear-gradient(135deg, ${theme.palette.primary['50']} 0%, ${theme.palette.background.paper} 100%)`,
            overflow: 'visible',
          }}
        >
          <CardContent sx={{ p: 3.5, '&:last-child': { pb: 3.5 } }}>
            <Grid container spacing={3} alignItems="center">
              <Grid item xs={12} md={7}>
                <Stack direction="row" spacing={3} alignItems="center">
                  <Avatar
                    sx={{
                      width: 72,
                      height: 72,
                      bgcolor: 'primary.main',
                      fontSize: '1.75rem',
                      fontWeight: 700,
                      boxShadow: '0 4px 14px rgba(0,0,0,0.08)',
                    }}
                  >
                    {data.employeeName?.charAt(0)?.toUpperCase() ?? '?'}
                  </Avatar>
                  <Box>
                    <Typography variant="h5" sx={{ fontWeight: 800, color: '#111827' }}>
                      {data.employeeName}
                    </Typography>
                    <Stack direction="row" spacing={1} alignItems="center" mt={0.5} flexWrap="wrap" gap={0.5}>
                      <Typography variant="body2" sx={{ fontFamily: 'monospace', fontWeight: 600, color: 'primary.main' }}>
                        {data.employeeCode}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">|</Typography>
                      <Typography variant="body2" color="text.secondary">{data.role ?? 'No Designation'}</Typography>
                      <Typography variant="body2" color="text.secondary">|</Typography>
                      <Typography variant="body2" color="text.secondary">{data.practice ?? 'No Practice'}</Typography>
                    </Stack>
                    <Stack direction="row" spacing={1.5} mt={1.5} flexWrap="wrap" gap={0.5}>
                      <Chip
                        label={data.active ? 'Active' : 'Inactive'}
                        size="small"
                        color={data.active ? 'success' : 'error'}
                        variant="filled"
                        sx={{ fontWeight: 600, height: 24, fontSize: '0.75rem' }}
                      />
                      <Chip
                        label={`${data.totalExperience} yrs`}
                        size="small"
                        variant="outlined"
                        color="primary"
                        sx={{ fontWeight: 600, height: 24, fontSize: '0.75rem' }}
                      />
                      <Chip
                        label={`${data.experienceRange}`}
                        size="small"
                        variant="outlined"
                        sx={{ fontWeight: 600, height: 24, fontSize: '0.75rem' }}
                      />
                    </Stack>
                  </Box>
                </Stack>
              </Grid>
              <Grid item xs={12} md={5}>
                <Stack direction="row" spacing={3} justifyContent={{ xs: 'flex-start', md: 'flex-end' }} flexWrap="wrap">
                  <Box textAlign="center">
                    <Typography variant="caption" sx={{ fontWeight: 500, color: '#6B7280' }}>Location</Typography>
                    <Typography variant="body2" sx={{ fontWeight: 600, color: '#111827' }}>{data.location ?? '-'}</Typography>
                  </Box>
                  <Box textAlign="center">
                    <Typography variant="caption" sx={{ fontWeight: 500, color: '#6B7280' }}>Allocation</Typography>
                    <Typography
                      variant="body1"
                      sx={{ fontWeight: 700, color: totalAllocated > 100 ? '#ef4444' : totalAllocated >= 100 ? '#f59e0b' : '#10b981' }}
                    >
                      {totalAllocated}%
                    </Typography>
                  </Box>
                  <Box textAlign="center">
                    <Typography variant="caption" sx={{ fontWeight: 500, color: '#6B7280' }}>Projects</Typography>
                    <Typography variant="body1" sx={{ fontWeight: 700, color: '#111827' }}>{data.projectAllocations.length}</Typography>
                  </Box>
                </Stack>
              </Grid>
            </Grid>
          </CardContent>
        </Card>

        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Tabs
            value={tabValue}
            onChange={handleTabChange}
            variant="scrollable"
            scrollButtons="auto"
            allowScrollButtonsMobile
            sx={{
              minHeight: 48,
              '& .MuiTab-root': {
                textTransform: 'none',
                fontWeight: 600,
                fontSize: '0.875rem',
                minHeight: 48,
                px: 3,
                color: '#6B7280',
                '&.Mui-selected': { color: 'primary.main' },
              },
              '& .MuiTabs-indicator': {
                height: 3,
                borderRadius: '3px 3px 0 0',
              },
            }}
          >
            <Tab icon={<PersonOutlineOutlinedIcon sx={{ fontSize: '1.1rem' }} />} iconPosition="start" label="Employee Information" />
            <Tab icon={<WorkspacePremiumOutlinedIcon sx={{ fontSize: '1.1rem' }} />} iconPosition="start" label="Experience Details" />
            <Tab icon={<AssignmentOutlinedIcon sx={{ fontSize: '1.1rem' }} />} iconPosition="start" label="Employment Details" />
            <Tab icon={<TimelineOutlinedIcon sx={{ fontSize: '1.1rem' }} />} iconPosition="start" label="Project Allocation Details" />
          </Tabs>
        </Box>

        {/* ============================================================ */}
        {/* TAB 1: Employee Information                                   */}
        {/* ============================================================ */}
        {tabValue === 0 && (
          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <Box
                sx={{
                  bgcolor: '#fff',
                  borderRadius: 3,
                  overflow: 'hidden',
                  boxShadow: '0 1px 4px rgba(0,0,0,.04)',
                  border: '1px solid',
                  borderColor: '#e5e7eb',
                  height: '100%',
                  position: 'relative',
                  '&::before': {
                    content: '""',
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    width: 4,
                    height: '100%',
                    bgcolor: '#3b82f6',
                  },
                }}
              >
                <Box sx={{ px: 3, pt: 2.5, pb: 0 }}>
                  <Stack direction="row" alignItems="center" spacing={1.5} mb={2} pb={1.5} sx={{ borderBottom: '2px solid #f3f4f6' }}>
                    <Box sx={{ width: 32, height: 32, borderRadius: 1.5, bgcolor: '#eff6ff', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                      <PersonOutlineOutlinedIcon sx={{ fontSize: 18, color: '#3b82f6' }} />
                    </Box>
                    <Typography sx={{ fontSize: 16, fontWeight: 700, color: '#111827' }}>
                      Personal Information
                    </Typography>
                  </Stack>
                </Box>
                <Box sx={{ px: 3, pb: 2.5 }}>
                  <Grid container spacing={1.5}>
                    <FieldTile label="Employee Code" value={data.employeeCode} />
                    <FieldTile label="Employee Name" value={data.employeeName} />
                    <FieldTile label="Email" value={data.email} />
                    <FieldTile label="Contact Number" value="N/A" />
                    <FieldTile label="Gender" value="N/A" />
                    <FieldTile label="Date of Birth" value="N/A" />
                  </Grid>
                </Box>
              </Box>
            </Grid>
            <Grid item xs={12} md={6}>
              <Box
                sx={{
                  bgcolor: '#fff',
                  borderRadius: 3,
                  overflow: 'hidden',
                  boxShadow: '0 1px 4px rgba(0,0,0,.04)',
                  border: '1px solid',
                  borderColor: '#e5e7eb',
                  height: '100%',
                  position: 'relative',
                  '&::before': {
                    content: '""',
                    position: 'absolute',
                    top: 0,
                    left: 0,
                    width: 4,
                    height: '100%',
                    bgcolor: '#8b5cf6',
                  },
                }}
              >
                <Box sx={{ px: 3, pt: 2.5, pb: 0 }}>
                  <Stack direction="row" alignItems="center" spacing={1.5} mb={2} pb={1.5} sx={{ borderBottom: '2px solid #f3f4f6' }}>
                    <Box sx={{ width: 32, height: 32, borderRadius: 1.5, bgcolor: '#f5f3ff', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                      <WorkspacePremiumOutlinedIcon sx={{ fontSize: 18, color: '#8b5cf6' }} />
                    </Box>
                    <Typography sx={{ fontSize: 16, fontWeight: 700, color: '#111827' }}>
                      Organization Information
                    </Typography>
                  </Stack>
                </Box>
                <Box sx={{ px: 3, pb: 2.5 }}>
                  <Grid container spacing={1.5}>
                    <FieldTile label="Designation" value={data.role} />
                    <FieldTile label="Practice" value={data.practice} />
                    <FieldTile label="Primary Skill" value={data.primarySkill} />
                    <FieldTile label="Secondary Skill" value={data.skill} />
                    <FieldTile label="Reporting Manager" value={data.l1Manager} />
                    <FieldTile label="Resource Manager" value={data.practiceHead} />
                    <FieldTile label="Work Location" value={data.location} />
                    <FieldTile label="Joining Date" value={formatDate(data.doj)} />
                  </Grid>
                </Box>
              </Box>
            </Grid>
          </Grid>
        )}

        {/* ============================================================ */}
        {/* TAB 2: Experience Details                                     */}
        {/* ============================================================ */}
        {tabValue === 1 && (
          <Stack spacing={3}>
            <Grid container spacing={2}>
              <Grid item xs={6} sm={3}>
                <SummaryStat label="Total Experience" value={`${data.totalExperience} yrs`} color="primary.main" />
              </Grid>
              <Grid item xs={6} sm={3}>
                <SummaryStat label="Relevant Experience" value={`${data.nvExperience} yrs`} color="secondary.main" />
              </Grid>
              <Grid item xs={6} sm={3}>
                <SummaryStat label="Prior Experience" value={`${data.priorExperience} yrs`} />
              </Grid>
              <Grid item xs={6} sm={3}>
                <SummaryStat label="Experience Range" value={data.experienceRange} color="success.main" />
              </Grid>
            </Grid>

            <Card
              elevation={0}
              sx={{
                borderRadius: 3,
                border: '1px solid',
                borderColor: 'divider',
                overflow: 'hidden',
              }}
            >
              <Box sx={{ px: 3.5, pt: 3, pb: 0 }}>
                <SectionHeader icon={<WorkspacePremiumOutlinedIcon fontSize="small" />} title="Experience Details" />
              </Box>
              <CardContent sx={{ px: 3.5, pb: 3.5, '&:last-child': { pb: 3.5 } }}>
                <Grid container spacing={4}>
                  <Grid item xs={12} md={6}>
                    <Stack spacing={0}>
                      <TimelineEntry
                        period="Present"
                        title={data.role ?? 'Software Engineer'}
                        subtitle="New Vision Software"
                        active
                      />
                      <TimelineEntry
                        period="Prior Experience"
                        title="Previous Experience"
                        subtitle={`${data.priorExperience} years of prior experience`}
                      />
                    </Stack>
                  </Grid>
                  <Grid item xs={12} md={6}>
                    <Stack spacing={3}>
                      <Box>
                        <Typography variant="caption" sx={{ fontWeight: 500, color: '#6B7280', mb: 1, display: 'block' }}>
                          Experience Breakdown
                        </Typography>
                        <Stack spacing={1.5}>
                          <Box>
                            <Stack direction="row" justifyContent="space-between" mb={0.5}>
                              <Typography variant="caption" color="text.secondary">Prior to NV</Typography>
                              <Typography variant="caption" sx={{ fontWeight: 700 }}>{data.priorExperience} yrs</Typography>
                            </Stack>
                            <LinearProgress
                              variant="determinate"
                              value={data.totalExperience > 0 ? (data.priorExperience / data.totalExperience) * 100 : 0}
                              sx={{ borderRadius: 2, height: 6, bgcolor: 'grey.100' }}
                              color="secondary"
                            />
                          </Box>
                          <Box>
                            <Stack direction="row" justifyContent="space-between" mb={0.5}>
                              <Typography variant="caption" color="text.secondary">In NV</Typography>
                              <Typography variant="caption" sx={{ fontWeight: 700 }}>{data.nvExperience} yrs</Typography>
                            </Stack>
                            <LinearProgress
                              variant="determinate"
                              value={data.totalExperience > 0 ? (data.nvExperience / data.totalExperience) * 100 : 0}
                              sx={{ borderRadius: 2, height: 6, bgcolor: 'grey.100' }}
                              color="primary"
                            />
                          </Box>
                        </Stack>
                      </Box>
                      <Divider />
                      <Box>
                        <Typography variant="caption" sx={{ fontWeight: 500, color: '#6B7280', mb: 1.5, display: 'block' }}>
                          Key Skills
                        </Typography>
                        {data.skill ? (
                          <Stack direction="row" spacing={0.5} flexWrap="wrap" useFlexGap>
                            {data.skill.split(',').map((s, i) => (
                              <Chip key={i} label={s.trim()} size="small" variant="outlined" color="info" sx={{ fontSize: '0.75rem', m: 0.25 }} />
                            ))}
                          </Stack>
                        ) : (
                          <Typography variant="body2" color="text.secondary">No skills recorded</Typography>
                        )}
                      </Box>
                    </Stack>
                  </Grid>
                </Grid>
              </CardContent>
            </Card>
          </Stack>
        )}

        {/* ============================================================ */}
        {/* TAB 3: Employment Details                                      */}
        {/* ============================================================ */}
        {tabValue === 2 && (
          <Card
            elevation={0}
            sx={{
              borderRadius: 3,
              border: '1px solid',
              borderColor: 'divider',
              overflow: 'hidden',
            }}
          >
            <Box sx={{ px: 3.5, pt: 3, pb: 0 }}>
              <SectionHeader icon={<AssignmentOutlinedIcon fontSize="small" />} title="Employment Details" />
            </Box>
            <CardContent sx={{ px: 3.5, pb: 3.5, '&:last-child': { pb: 3.5 } }}>
              <Grid container spacing={3}>
                <Grid item xs={12} md={6}>
                  <Stack spacing={2.5}>
                    <Typography variant="subtitle2" sx={{ fontWeight: 700, color: '#111827' }}>Employment Status</Typography>
                    <DLRow label="Employment Status" value={data.status} />
                    <DLRow
                      label="Employee Type"
                      value={
                        <Chip
                          label={data.fteConsultant ?? '-'}
                          size="small"
                          color={data.fteConsultant === 'FTE' ? 'primary' : 'default'}
                          variant="outlined"
                          sx={{ fontWeight: 600, height: 24, fontSize: '0.75rem' }}
                        />
                      }
                    />
                    <DLRow label="Work Model" value="N/A" />
                    <DLRow label="Notice Period" value="N/A" />
                    <DLRow
                      label="Billing Status"
                      value={
                        <Chip
                          label={data.billable ?? '-'}
                          size="small"
                          color={data.billable === 'Yes' ? 'success' : 'default'}
                          variant="outlined"
                          sx={{ fontWeight: 600, height: 24, fontSize: '0.75rem' }}
                        />
                      }
                    />
                    <DLRow label="Department" value={data.practice} />
                    <DLRow label="Business Unit" value={data.practice} />
                  </Stack>
                </Grid>
                <Grid item xs={12} md={6}>
                  <Stack spacing={2.5}>
                    <Typography variant="subtitle2" sx={{ fontWeight: 700, color: '#111827' }}>Employment Timeline</Typography>
                    <DLRow icon={<CalendarTodayOutlinedIcon fontSize="inherit" />} label="Employment Start Date" value={formatDate(data.doj)} />
                    <DLRow label="Employment End Date" value="N/A" />
                    <DLRow label="Last Working Date" value="N/A" />
                    <DLRow icon={<CalendarTodayOutlinedIcon fontSize="inherit" />} label="Joining Date" value={formatDate(data.doj)} />
                    <DLRow label="Created Date" value={formatDate(data.doj)} />
                    <DLRow label="Modified Date" value="N/A" />
                  </Stack>
                </Grid>
              </Grid>
            </CardContent>
          </Card>
        )}

        {/* ============================================================ */}
        {/* TAB 4: Project Allocation Details                              */}
        {/* ============================================================ */}
        {tabValue === 3 && (
          <Stack spacing={3}>
            <Grid container spacing={2}>
              <Grid item xs={6} sm={3}>
                <SummaryStat
                  label="Total Allocation"
                  value={`${totalAllocated}%`}
                  color={totalAllocated > 100 ? '#ef4444' : totalAllocated >= 100 ? '#f59e0b' : '#10b981'}
                />
              </Grid>
              <Grid item xs={6} sm={3}>
                <SummaryStat
                  label="Available Capacity"
                  value={`${availableCapacity}%`}
                  color={availableCapacity > 0 ? '#10b981' : '#ef4444'}
                />
              </Grid>
              <Grid item xs={6} sm={3}>
                <SummaryStat label="Active Projects" value={data.projectAllocations.length} color="primary.main" />
              </Grid>
              <Grid item xs={6} sm={3}>
                <SummaryStat
                  label="Utilisation"
                  value={totalAllocated > 100 ? 'Overallocated' : totalAllocated >= 80 ? 'Highly Utilised' : 'Normal'}
                  color={totalAllocated > 100 ? '#ef4444' : totalAllocated >= 80 ? '#f59e0b' : '#10b981'}
                />
              </Grid>
            </Grid>

            <Card
              elevation={0}
              sx={{
                borderRadius: 3,
                border: '1px solid',
                borderColor: 'divider',
                overflow: 'hidden',
              }}
            >
              <Box sx={{ px: 3.5, pt: 3, pb: 0 }}>
                <SectionHeader icon={<TimelineOutlinedIcon fontSize="small" />} title="Project Allocations" />
              </Box>
              <CardContent sx={{ px: 3.5, pb: 3.5, '&:last-child': { pb: 3.5 } }}>
                {data.projectAllocations.length === 0 ? (
                  <Box textAlign="center" py={4}>
                    <Typography color="text.secondary">No Active Allocations</Typography>
                  </Box>
                ) : (
                  <>
                    <TableContainer component={Paper} elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
                      <Table size="small">
                        <TableHead>
                          <TableRow>
                            <TableCell sx={{ fontWeight: 700, color: '#6B7280', bgcolor: 'grey.50' }}>
                              <TableSortLabel active={sortKey === 'project'} direction={sortKey === 'project' ? sortDir : 'asc'} onClick={() => handleSort('project')}>
                                Project Name
                              </TableSortLabel>
                            </TableCell>
                            <TableCell sx={{ fontWeight: 700, color: '#6B7280', bgcolor: 'grey.50' }}>Project Code</TableCell>
                            <TableCell sx={{ fontWeight: 700, color: '#6B7280', bgcolor: 'grey.50' }}>
                              <TableSortLabel active={sortKey === 'client'} direction={sortKey === 'client' ? sortDir : 'asc'} onClick={() => handleSort('client')}>
                                Client Name
                              </TableSortLabel>
                            </TableCell>
                            <TableCell sx={{ fontWeight: 700, color: '#6B7280', bgcolor: 'grey.50' }}>
                              <TableSortLabel active={sortKey === 'allocationPercentage'} direction={sortKey === 'allocationPercentage' ? sortDir : 'asc'} onClick={() => handleSort('allocationPercentage')}>
                                Allocation %
                              </TableSortLabel>
                            </TableCell>
                            <TableCell sx={{ fontWeight: 700, color: '#6B7280', bgcolor: 'grey.50' }}>
                              <TableSortLabel active={sortKey === 'startDate'} direction={sortKey === 'startDate' ? sortDir : 'asc'} onClick={() => handleSort('startDate')}>
                                Start Date
                              </TableSortLabel>
                            </TableCell>
                            <TableCell sx={{ fontWeight: 700, color: '#6B7280', bgcolor: 'grey.50' }}>
                              <TableSortLabel active={sortKey === 'endDate'} direction={sortKey === 'endDate' ? sortDir : 'asc'} onClick={() => handleSort('endDate')}>
                                End Date
                              </TableSortLabel>
                            </TableCell>
                            <TableCell sx={{ fontWeight: 700, color: '#6B7280', bgcolor: 'grey.50' }}>Status</TableCell>
                            <TableCell sx={{ fontWeight: 700, color: '#6B7280', bgcolor: 'grey.50' }}>Duration</TableCell>
                          </TableRow>
                        </TableHead>
                        <TableBody>
                          {paginatedAllocations.map((pa, i) => {
                            const isOver = (pa.allocationPercentage ?? 0) > 100;
                            return (
                              <TableRow
                                key={i}
                                hover
                                sx={{ '&:hover': { bgcolor: 'grey.25' }, bgcolor: isOver ? 'error.50' : 'inherit' }}
                              >
                                <TableCell>
                                  <Typography variant="body2" sx={{ fontWeight: 600, color: '#111827' }}>{pa.project ?? '-'}</Typography>
                                </TableCell>
                                <TableCell>
                                  <Typography variant="body2" sx={{ fontFamily: 'monospace', fontWeight: 600, color: '#6B7280' }}>
                                    {pa.projectCode ?? '-'}
                                  </Typography>
                                </TableCell>
                                <TableCell>{pa.client ?? '-'}</TableCell>
                                <TableCell>
                                  <Stack direction="row" alignItems="center" spacing={1}>
                                    <Typography
                                      variant="body2"
                                      sx={{ fontWeight: 700, color: isOver ? '#ef4444' : (pa.allocationPercentage ?? 0) >= 100 ? '#f59e0b' : '#10b981' }}
                                    >
                                      {pa.allocationPercentage != null ? `${pa.allocationPercentage}%` : '-'}
                                    </Typography>
                                    {isOver && <Chip label="Over" size="small" color="error" variant="filled" sx={{ height: 20, fontSize: '0.65rem' }} />}
                                  </Stack>
                                </TableCell>
                                <TableCell>
                                  <Typography variant="body2" color="text.secondary">{formatDate(pa.startDate)}</Typography>
                                </TableCell>
                                <TableCell>
                                  <Typography variant="body2" color="text.secondary">{formatDate(pa.endDate)}</Typography>
                                </TableCell>
                                <TableCell>
                                  <Chip
                                    label={pa.projectStatus ?? '-'}
                                    size="small"
                                    color={projectStatusColors[pa.projectStatus ?? ''] ?? 'default'}
                                    variant="outlined"
                                    sx={{ fontWeight: 600, fontSize: '0.75rem' }}
                                  />
                                </TableCell>
                                <TableCell>
                                  <Typography variant="body2" color="text.secondary">{pa.durationInProject ?? '-'}</Typography>
                                </TableCell>
                              </TableRow>
                            );
                          })}
                        </TableBody>
                      </Table>
                    </TableContainer>
                    <TablePagination
                      component="div"
                      count={sortedAllocations.length}
                      page={allocationPage}
                      onPageChange={(_, p) => setAllocationPage(p)}
                      rowsPerPage={allocationRowsPerPage}
                      onRowsPerPageChange={(e) => { setAllocationRowsPerPage(Number(e.target.value)); setAllocationPage(0); }}
                      rowsPerPageOptions={[5, 10, 25]}
                    />
                  </>
                )}
              </CardContent>
            </Card>
          </Stack>
        )}
      </Stack>
    </PageContainer>
  );
}

function FieldTile({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <Grid item xs={12} sm={6}>
      <Box
        sx={{
          bgcolor: '#f9fafb',
          borderRadius: 1.5,
          px: 2,
          py: 1.5,
          border: '1px solid',
          borderColor: '#f3f4f6',
          transition: 'all 0.15s',
          '&:hover': { borderColor: '#d1d5db', bgcolor: '#fff' },
        }}
      >
        <Typography sx={{ fontSize: 12, fontWeight: 500, color: '#9ca3af', mb: 0.25, textTransform: 'uppercase', letterSpacing: 0.4 }}>
          {label}
        </Typography>
        <Typography sx={{ fontSize: 14, fontWeight: 600, color: '#111827' }}>
          {value ?? '-'}
        </Typography>
      </Box>
    </Grid>
  );
}

function SummaryStat({ label, value, color }: { label: string; value: React.ReactNode; color?: string }) {
  return (
    <Card
      elevation={0}
      sx={{
        borderRadius: 2,
        border: '1px solid',
        borderColor: 'divider',
        p: 2.5,
        textAlign: 'center',
      }}
    >
      <Typography variant="caption" sx={{ fontWeight: 500, color: '#6B7280', display: 'block', mb: 0.5 }}>
        {label}
      </Typography>
      <Typography variant="h5" sx={{ fontWeight: 800, color: color ?? '#111827' }}>
        {value}
      </Typography>
    </Card>
  );
}

function DLRow({ label, value, icon }: { label: string; value: React.ReactNode; icon?: React.ReactNode }) {
  return (
    <Stack direction="row" spacing={1} alignItems="center">
      <Box sx={{ minWidth: 160 }}>
        <Stack direction="row" alignItems="center" spacing={0.5}>
          {icon && <Box sx={{ color: '#6B7280', display: 'flex', fontSize: '0.85rem' }}>{icon}</Box>}
          <Typography variant="body2" sx={{ fontWeight: 500, color: '#6B7280' }}>{label}</Typography>
        </Stack>
      </Box>
      <Typography variant="body2" sx={{ fontWeight: 600, color: '#111827' }}>:</Typography>
      <Typography variant="body2" sx={{ fontWeight: 600, color: '#111827' }}>{value ?? '-'}</Typography>
    </Stack>
  );
}

function TimelineEntry({ period, title, subtitle, active }: { period: string; title: string; subtitle: string; active?: boolean }) {
  return (
    <Stack direction="row" spacing={2}>
      <Stack alignItems="center" spacing={0.5} sx={{ minWidth: 28 }}>
        <Box
          sx={{
            width: 12,
            height: 12,
            borderRadius: '50%',
            bgcolor: active ? 'primary.main' : 'grey.300',
            border: active ? '3px solid' : 'none',
            borderColor: 'primary.100',
            mt: 0.5,
          }}
        />
        <Box sx={{ width: 2, flex: 1, bgcolor: 'grey.200', minHeight: 60 }} />
      </Stack>
      <Box pb={3}>
        <Typography variant="caption" sx={{ fontWeight: 500, color: '#6B7280', display: 'block', mb: 0.25 }}>
          {period}
        </Typography>
        <Typography variant="body2" sx={{ fontWeight: 700, color: '#111827' }}>{title}</Typography>
        <Typography variant="body2" color="text.secondary">{subtitle}</Typography>
      </Box>
    </Stack>
  );
}
