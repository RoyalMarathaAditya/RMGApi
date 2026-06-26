import { useEffect, useMemo, useState, SyntheticEvent } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Button, Chip, CircularProgress, Tab, Tabs,
  Typography, Alert, Table, TableBody, TableCell,
  TableContainer, TableHead, TableRow, Paper, TablePagination, TableSortLabel,
} from '@mui/material';
import ArrowBackOutlinedIcon from '@mui/icons-material/ArrowBackOutlined';
import PersonOutlineOutlinedIcon from '@mui/icons-material/PersonOutlineOutlined';
import WorkspacePremiumOutlinedIcon from '@mui/icons-material/WorkspacePremiumOutlined';
import AssignmentOutlinedIcon from '@mui/icons-material/AssignmentOutlined';
import TimelineOutlinedIcon from '@mui/icons-material/TimelineOutlined';
import CalendarTodayOutlinedIcon from '@mui/icons-material/CalendarTodayOutlined';
import BadgeOutlinedIcon from '@mui/icons-material/BadgeOutlined';
import EmailOutlinedIcon from '@mui/icons-material/EmailOutlined';
import PhoneOutlinedIcon from '@mui/icons-material/PhoneOutlined';
import WcOutlinedIcon from '@mui/icons-material/WcOutlined';
import BloodtypeOutlinedIcon from '@mui/icons-material/BloodtypeOutlined';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import PsychologyOutlinedIcon from '@mui/icons-material/PsychologyOutlined';
import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import AccountBalanceOutlinedIcon from '@mui/icons-material/AccountBalanceOutlined';
import HistoryEduOutlinedIcon from '@mui/icons-material/HistoryEduOutlined';
import VisibilityOutlinedIcon from '@mui/icons-material/VisibilityOutlined';
import ProfileHeader from '../../employees/components/ProfileHeader';
import InfoSection from '../../employees/components/InfoSection';
import InfoField from '../../employees/components/InfoField';
import InfoGrid from '../../employees/components/InfoGrid';
import ProjectDrawer from '../components/ProjectDrawer';
import { allocationService } from '../services/allocationService';
import type { EmployeeResourceDetailsDto, ProjectAllocationDetailDto } from '../types/allocation';

function formatDate(dateStr: string | null | undefined): string {
  if (!dateStr) return '—';
  const d = new Date(dateStr);
  return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
}

const projectStatusColors: Record<string, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
  Billable: 'success', 'Non-Billable': 'warning', Shadow: 'info',
  Active: 'success', Planned: 'info', Completed: 'default',
  Released: 'warning', Cancelled: 'error',
};

type SortKey = 'project' | 'allocationPercentage' | 'startDate' | 'endDate' | 'client';
type SortDir = 'asc' | 'desc';

const sectionConfig = {
  personal: {
    icon: <PersonOutlineOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
    title: 'Personal Information',
    subtitle: 'Basic employee details and contact information',
    headerBg: '#EFF6FF', accentColor: '#2563EB',
  },
  organization: {
    icon: <BadgeOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
    title: 'Organization Information',
    subtitle: 'Role, department and reporting structure',
    headerBg: '#F5F3FF', accentColor: '#7C3AED',
  },
  employment: {
    icon: <AssignmentOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
    title: 'Employment Information',
    subtitle: 'Employment type, status and timeline',
    headerBg: '#ECFDF5', accentColor: '#059669',
  },
  experience: {
    icon: <WorkspacePremiumOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
    title: 'Experience Information',
    subtitle: 'Skills, experience breakdown and certifications',
    headerBg: '#FFF7ED', accentColor: '#D97706',
  },
  allocation: {
    icon: <TimelineOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
    title: 'Allocation Information',
    subtitle: 'Project allocations and capacity summary',
    headerBg: '#ECFEFF', accentColor: '#0891B2',
  },
};

const tabs = ['Employee Information', 'Experience Details', 'Employment Details', 'Project Allocation'];

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
  const [drawerProject, setDrawerProject] = useState<ProjectAllocationDetailDto | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);

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

  const handleTabChange = (_: SyntheticEvent, newValue: number) => setTabValue(newValue);

  const totalAllocated = useMemo(() => {
    if (!data) return 0;
    return data.projectAllocations.reduce((sum, pa) => sum + (pa.allocationPercentage ?? 0), 0);
  }, [data]);

  const availableCapacity = Math.max(0, 100 - totalAllocated);

  const handleSort = (key: SortKey) => {
    if (sortKey === key) setSortDir((prev) => (prev === 'asc' ? 'desc' : 'asc'));
    else { setSortKey(key); setSortDir('asc'); }
  };

  const sortedAllocations = useMemo(() => {
    if (!data) return [];
    const list = [...data.projectAllocations];
    list.sort((a, b) => {
      let cmp = 0;
      switch (sortKey) {
        case 'project': cmp = (a.project ?? '').localeCompare(b.project ?? ''); break;
        case 'allocationPercentage': cmp = (a.allocationPercentage ?? 0) - (b.allocationPercentage ?? 0); break;
        case 'startDate': cmp = (a.startDate ?? '').localeCompare(b.startDate ?? ''); break;
        case 'endDate': cmp = (a.endDate ?? '').localeCompare(b.endDate ?? ''); break;
        case 'client': cmp = (a.client ?? '').localeCompare(b.client ?? ''); break;
      }
      return sortDir === 'asc' ? cmp : -cmp;
    });
    return list;
  }, [data, sortKey, sortDir]);

  const paginatedAllocations = useMemo(() => {
    return sortedAllocations.slice(allocationPage * allocationRowsPerPage, allocationPage * allocationRowsPerPage + allocationRowsPerPage);
  }, [sortedAllocations, allocationPage, allocationRowsPerPage]);

  if (loading) {
    return (
      <Box sx={{ bgcolor: '#F8FAFC', minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 2, flexDirection: 'column' }}>
        <CircularProgress size={40} sx={{ color: '#2563EB' }} />
        <Typography sx={{ fontSize: 14, color: '#6B7280' }}>Loading employee details...</Typography>
      </Box>
    );
  }

  if (error) {
    return (
      <Box sx={{ bgcolor: '#F8FAFC', minHeight: '100vh', p: 3 }}>
        <Alert severity="error" sx={{ borderRadius: 2, mb: 2 }}>{error}</Alert>
        <Button startIcon={<ArrowBackOutlinedIcon />} onClick={() => navigate(-1)} sx={{ textTransform: 'none', fontWeight: 600 }}>Go Back</Button>
      </Box>
    );
  }

  if (!data) {
    return (
      <Box sx={{ bgcolor: '#F8FAFC', minHeight: '100vh', p: 3 }}>
        <Box sx={{ maxWidth: 480, mx: 'auto', mt: 8, textAlign: 'center' }}>
          <Typography sx={{ fontSize: 20, fontWeight: 700, color: '#111827', mb: 1 }}>Employee Not Found</Typography>
          <Typography sx={{ fontSize: 14, color: '#6B7280', mb: 3 }}>The requested employee does not exist or has been removed.</Typography>
          <Button startIcon={<ArrowBackOutlinedIcon />} onClick={() => navigate('/rmg')} sx={{ textTransform: 'none', fontWeight: 600 }}>Back to Dashboard</Button>
        </Box>
      </Box>
    );
  }

  return (
    <Box sx={{ bgcolor: '#F8FAFC', minHeight: '100vh' }}>
      <Box sx={{ maxWidth: 1600, mx: 'auto', px: '24px', pt: '16px', pb: '24px' }}>
        <Button
          startIcon={<ArrowBackOutlinedIcon />}
          onClick={() => navigate('/rmg')}
          size="small"
          sx={{ textTransform: 'none', fontWeight: 600, fontSize: 12, color: '#6B7280', p: 0, minWidth: 0, mb: 1.5, '&:hover': { color: '#2563EB', bgcolor: 'transparent' } }}
        >
          Back to Dashboard
        </Button>

        <Typography sx={{ fontSize: 36, fontWeight: 700, color: '#111827', lineHeight: 1.15, mb: 2 }}>
          Resource Allocation Preview
        </Typography>

        <ProfileHeader data={data} totalAllocated={totalAllocated} />

        <Box sx={{ mt: 2, borderBottom: '1px solid #E5E7EB', bgcolor: '#FFF', borderRadius: '12px 12px 0 0', border: '1px solid #E5E7EB', borderBottom: 'none' }}>
          <Tabs
            value={tabValue}
            onChange={handleTabChange}
            variant="scrollable"
            scrollButtons="auto"
            sx={{
              minHeight: 48, px: 1.5,
              '& .MuiTab-root': {
                textTransform: 'none', fontWeight: 600, fontSize: '0.8125rem',
                minHeight: 48, px: 2.5, py: 0, color: '#6B7280',
                borderRadius: '8px 8px 0 0', transition: 'all 200ms ease',
                '&.Mui-selected': { color: '#2563EB', bgcolor: '#EFF6FF', fontWeight: 700 },
                '&:hover:not(.Mui-selected)': { bgcolor: '#F8FAFC', color: '#374151' },
              },
              '& .MuiTabs-indicator': { height: 3, borderRadius: '3px 3px 0 0', bgcolor: '#2563EB' },
            }}
          >
            {tabs.map((tab) => (<Tab key={tab} label={tab} />))}
          </Tabs>
        </Box>

        {/* ── TAB 0: EMPLOYEE INFORMATION ── */}
        {tabValue === 0 && (
          <Box sx={{ mt: '20px', display: 'flex', flexDirection: 'column', gap: '20px' }}>
            <InfoSection {...sectionConfig.personal}>
              <InfoGrid>
                <InfoField icon={<BadgeOutlinedIcon sx={{ fontSize: 16 }} />} label="Employee Code" value={data.employeeCode} />
                <InfoField icon={<PersonOutlineOutlinedIcon sx={{ fontSize: 16 }} />} label="Full Name" value={data.employeeName} />
                <InfoField icon={<EmailOutlinedIcon sx={{ fontSize: 16 }} />} label="Email" value={data.email} />
                <InfoField icon={<PhoneOutlinedIcon sx={{ fontSize: 16 }} />} label="Contact" value="—" />
                <InfoField icon={<WcOutlinedIcon sx={{ fontSize: 16 }} />} label="Gender" value="—" />
                <InfoField icon={<CalendarTodayOutlinedIcon sx={{ fontSize: 16 }} />} label="Date of Birth" value="—" />
                <InfoField icon={<BloodtypeOutlinedIcon sx={{ fontSize: 16 }} />} label="Blood Group" value="—" />
                <InfoField icon={<WcOutlinedIcon sx={{ fontSize: 16 }} />} label="Marital Status" value="—" />
              </InfoGrid>
            </InfoSection>

            <InfoSection {...sectionConfig.organization}>
              <InfoGrid>
                <InfoField icon={<BadgeOutlinedIcon sx={{ fontSize: 16 }} />} label="Designation" value={data.role ?? '—'} />
                <InfoField icon={<AccountBalanceOutlinedIcon sx={{ fontSize: 16 }} />} label="Department" value={data.practice ?? '—'} />
                <InfoField icon={<AccountBalanceOutlinedIcon sx={{ fontSize: 16 }} />} label="Business Unit" value={data.practice ?? '—'} />
                <InfoField icon={<LocationOnOutlinedIcon sx={{ fontSize: 16 }} />} label="Office" value={data.location ?? '—'} />
                <InfoField icon={<PsychologyOutlinedIcon sx={{ fontSize: 16 }} />} label="Primary Skill" value={data.primarySkill ?? '—'} />
                <InfoField icon={<PsychologyOutlinedIcon sx={{ fontSize: 16 }} />} label="Secondary Skills" value={data.skill ?? '—'} colSpan={2} />
                <InfoField icon={<GroupsOutlinedIcon sx={{ fontSize: 16 }} />} label="Reporting Manager" value={data.l1Manager ?? '—'} />
                <InfoField icon={<GroupsOutlinedIcon sx={{ fontSize: 16 }} />} label="Resource Manager" value={data.practiceHead ?? '—'} />
                <InfoField icon={<LocationOnOutlinedIcon sx={{ fontSize: 16 }} />} label="Work Location" value={data.location ?? '—'} />
                <InfoField icon={<CalendarTodayOutlinedIcon sx={{ fontSize: 16 }} />} label="Joining Date" value={formatDate(data.doj)} />
                <InfoField icon={<HistoryEduOutlinedIcon sx={{ fontSize: 16 }} />} label="Practice" value={data.practice ?? '—'} />
                <InfoField icon={<HistoryEduOutlinedIcon sx={{ fontSize: 16 }} />} label="Sub Practice" value={data.subPractice ?? '—'} />
              </InfoGrid>
            </InfoSection>
          </Box>
        )}

        {/* ── TAB 1: EXPERIENCE DETAILS ── */}
        {tabValue === 1 && (
          <Box sx={{ mt: '20px' }}>
            <InfoSection {...sectionConfig.experience}>
              <InfoGrid>
                <InfoField icon={<WorkspacePremiumOutlinedIcon sx={{ fontSize: 16 }} />} label="Total Experience" value={`${data.totalExperience} yrs`} />
                <InfoField icon={<WorkspacePremiumOutlinedIcon sx={{ fontSize: 16 }} />} label="NV Experience" value={`${data.nvExperience} yrs`} />
                <InfoField icon={<WorkspacePremiumOutlinedIcon sx={{ fontSize: 16 }} />} label="Prior Experience" value={`${data.priorExperience} yrs`} />
                <InfoField icon={<WorkspacePremiumOutlinedIcon sx={{ fontSize: 16 }} />} label="Experience Range" value={data.experienceRange} />
                <InfoField icon={<PsychologyOutlinedIcon sx={{ fontSize: 16 }} />} label="Primary Skill" value={data.primarySkill ?? '—'} />
                <InfoField icon={<PsychologyOutlinedIcon sx={{ fontSize: 16 }} />} label="Key Skills" value={data.skill ?? '—'} colSpan={3} />
              </InfoGrid>
            </InfoSection>
          </Box>
        )}

        {/* ── TAB 2: EMPLOYMENT DETAILS ── */}
        {tabValue === 2 && (
          <Box sx={{ mt: '20px' }}>
            <InfoSection {...sectionConfig.employment}>
              <InfoGrid>
                <InfoField icon={<AssignmentOutlinedIcon sx={{ fontSize: 16 }} />} label="Employment Status" value={data.status ?? '—'} />
                <InfoField icon={<AssignmentOutlinedIcon sx={{ fontSize: 16 }} />} label="Employee Type" value={data.fteConsultant ?? '—'} />
                <InfoField icon={<LocationOnOutlinedIcon sx={{ fontSize: 16 }} />} label="Work Model" value="—" />
                <InfoField icon={<HistoryEduOutlinedIcon sx={{ fontSize: 16 }} />} label="Notice Period" value="—" />
                <InfoField icon={<CalendarTodayOutlinedIcon sx={{ fontSize: 16 }} />} label="Joining Date" value={formatDate(data.doj)} />
                <InfoField icon={<CalendarTodayOutlinedIcon sx={{ fontSize: 16 }} />} label="Last Working Day" value="—" />
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Billing Status" value={data.billable ?? '—'} />
                <InfoField icon={<AccountBalanceOutlinedIcon sx={{ fontSize: 16 }} />} label="Department" value={data.practice ?? '—'} />
              </InfoGrid>
            </InfoSection>
          </Box>
        )}

        {/* ── TAB 3: PROJECT ALLOCATION ── */}
        {tabValue === 3 && (
          <Box sx={{ mt: '20px', display: 'flex', flexDirection: 'column', gap: '20px' }}>
            <InfoSection {...sectionConfig.allocation}>
              <InfoGrid>
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Total Allocation" value={`${totalAllocated}%`} />
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Available Capacity" value={`${availableCapacity}%`} />
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Active Projects" value={data.projectAllocations.length} />
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Utilisation" value={totalAllocated > 100 ? 'Overallocated' : totalAllocated >= 80 ? 'High' : 'Normal'} />
              </InfoGrid>
            </InfoSection>

            {/* Allocations Table */}
            <Box sx={{ borderRadius: '14px', border: '1px solid #E5E7EB', overflow: 'hidden', bgcolor: '#FFF', boxShadow: '0 1px 3px rgba(0,0,0,.04)' }}>
              <Box sx={{ px: '24px', py: 2, bgcolor: '#F8FAFC', borderBottom: '1px solid #E5E7EB', display: 'flex', alignItems: 'center', gap: 1.5 }}>
                <TimelineOutlinedIcon sx={{ fontSize: '1.1rem', color: '#0891B2' }} />
                <Typography sx={{ fontSize: 15, fontWeight: 600, color: '#111827' }}>Project Allocations</Typography>
                <Typography sx={{ fontSize: 12, fontWeight: 400, color: '#6B7280', ml: 'auto' }}>{data.projectAllocations.length} record(s)</Typography>
              </Box>

              {data.projectAllocations.length === 0 ? (
                <Box sx={{ textAlign: 'center', py: 6 }}>
                  <Typography sx={{ fontSize: 14, color: '#6B7280' }}>No Active Allocations</Typography>
                </Box>
              ) : (
                <>
                  <TableContainer>
                    <Table size="small">
                      <TableHead>
                        <TableRow>
                          {[
                            { key: 'project', label: 'Project Name' },
                            { key: null as any, label: 'Project Code' },
                            { key: 'client', label: 'Client Name' },
                            { key: 'allocationPercentage', label: 'Allocation %' },
                            { key: 'startDate', label: 'Start Date' },
                            { key: 'endDate', label: 'End Date' },
                            { key: null as any, label: 'Status' },
                            { key: null as any, label: 'Duration' },
                            { key: null as any, label: '' },
                          ].map((col) => (
                            <TableCell
                              key={col.label}
                              sx={{
                                fontWeight: 600, fontSize: '0.75rem', color: '#6B7280',
                                textTransform: 'uppercase', letterSpacing: '0.4px',
                                bgcolor: '#F8FAFC', borderBottom: '1px solid #E5E7EB', py: 1.5,
                              }}
                            >
                              {col.key ? (
                                <TableSortLabel
                                  active={sortKey === col.key}
                                  direction={sortKey === col.key ? sortDir : 'asc'}
                                  onClick={() => handleSort(col.key)}
                                >
                                  {col.label}
                                </TableSortLabel>
                              ) : col.label}
                            </TableCell>
                          ))}
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {paginatedAllocations.map((pa, i) => {
                          const isOver = (pa.allocationPercentage ?? 0) > 100;
                          return (
                            <TableRow
                              key={i}
                              hover
                              sx={{ '&:hover': { bgcolor: '#F8FBFF' }, '&:last-child td': { borderBottom: 'none' } }}
                            >
                              <TableCell sx={{ borderBottom: '1px solid #F3F4F6', py: 1.5 }}>
                                <Typography sx={{ fontSize: 14, fontWeight: 600, color: '#111827' }}>{pa.project ?? '—'}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: '1px solid #F3F4F6', py: 1.5 }}>
                                <Typography sx={{ fontSize: 13, fontWeight: 600, color: '#6B7280', fontFamily: 'monospace' }}>{pa.projectCode ?? '—'}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: '1px solid #F3F4F6', py: 1.5 }}>
                                <Typography sx={{ fontSize: 13, color: '#374151' }}>{pa.client ?? '—'}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: '1px solid #F3F4F6', py: 1.5 }}>
                                <Typography sx={{ fontSize: 14, fontWeight: 700, color: isOver ? '#DC2626' : (pa.allocationPercentage ?? 0) >= 100 ? '#F59E0B' : '#16A34A' }}>
                                  {pa.allocationPercentage != null ? `${pa.allocationPercentage}%` : '—'}
                                </Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: '1px solid #F3F4F6', py: 1.5 }}>
                                <Typography sx={{ fontSize: 13, color: '#374151' }}>{formatDate(pa.startDate)}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: '1px solid #F3F4F6', py: 1.5 }}>
                                <Typography sx={{ fontSize: 13, color: '#374151' }}>{formatDate(pa.endDate)}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: '1px solid #F3F4F6', py: 1.5 }}>
                                <Chip
                                  label={pa.projectStatus ?? '—'}
                                  size="small"
                                  color={projectStatusColors[pa.projectStatus ?? ''] ?? 'default'}
                                  variant="outlined"
                                  sx={{ fontWeight: 600, fontSize: '0.7rem', height: 22, borderRadius: '999px' }}
                                />
                              </TableCell>
                              <TableCell sx={{ borderBottom: '1px solid #F3F4F6', py: 1.5 }}>
                                <Typography sx={{ fontSize: 13, color: '#374151' }}>{pa.durationInProject ?? '—'}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: '1px solid #F3F4F6', py: 1.5 }}>
                                <Button
                                  size="small"
                                  startIcon={<VisibilityOutlinedIcon sx={{ fontSize: 14 }} />}
                                  onClick={() => { setDrawerProject(pa); setDrawerOpen(true); }}
                                  sx={{ textTransform: 'none', fontWeight: 600, fontSize: 12, color: '#2563EB', minWidth: 0, '&:hover': { bgcolor: '#EFF6FF' } }}
                                >
                                  View
                                </Button>
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
                    sx={{ borderTop: '1px solid #E5E7EB', fontSize: '0.8125rem' }}
                  />
                </>
              )}
            </Box>
          </Box>
        )}
      </Box>

      <ProjectDrawer
        open={drawerOpen}
        onClose={() => setDrawerOpen(false)}
        project={drawerProject}
      />
    </Box>
  );
}
