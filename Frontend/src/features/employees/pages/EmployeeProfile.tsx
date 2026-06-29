import { useEffect, useMemo, useState, SyntheticEvent } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Button,
  CircularProgress,
  Tab,
  Tabs,
  Typography,
  Alert,
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
import WorkOutlineOutlinedIcon from '@mui/icons-material/WorkOutlineOutlined';
import PsychologyOutlinedIcon from '@mui/icons-material/PsychologyOutlined';
import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import AccountBalanceOutlinedIcon from '@mui/icons-material/AccountBalanceOutlined';
import HistoryEduOutlinedIcon from '@mui/icons-material/HistoryEduOutlined';

import ProfileHeader from '../components/ProfileHeader';
import InfoSection from '../components/InfoSection';
import InfoField from '../components/InfoField';
import InfoGrid from '../components/InfoGrid';
import { allocationService } from '../../rmg/services/allocationService';
import type { EmployeeResourceDetailsDto } from '../../rmg/types/allocation';

function formatDate(dateStr: string | null | undefined): string {
  if (!dateStr) return '—';
  const d = new Date(dateStr);
  return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
}

const sectionConfig = {
  personal: {
    icon: <PersonOutlineOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
    title: 'Personal Information',
    subtitle: 'Basic employee details and contact information',
    headerBg: '#EFF6FF',
    accentColor: '#2563EB',
  },
  organization: {
    icon: <BadgeOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
    title: 'Organization Information',
    subtitle: 'Role, department and reporting structure',
    headerBg: '#F5F3FF',
    accentColor: '#7C3AED',
  },
  employment: {
    icon: <AssignmentOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
    title: 'Employment Information',
    subtitle: 'Employment type, status and timeline',
    headerBg: '#ECFDF5',
    accentColor: '#059669',
  },
  experience: {
    icon: <WorkspacePremiumOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
    title: 'Experience Information',
    subtitle: 'Skills, experience breakdown and certifications',
    headerBg: '#FFF7ED',
    accentColor: '#D97706',
  },
  allocation: {
    icon: <TimelineOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
    title: 'Allocation Information',
    subtitle: 'Project allocations and capacity summary',
    headerBg: '#ECFEFF',
    accentColor: '#0891B2',
  },
};

const tabs = ['Employee Information', 'Experience', 'Employment', 'Allocation History', 'Documents'];

export default function EmployeeProfile() {
  const { employeeId } = useParams<{ employeeId: string }>();
  const navigate = useNavigate();
  const [data, setData] = useState<EmployeeResourceDetailsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [tabValue, setTabValue] = useState(0);

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
        <Button startIcon={<ArrowBackOutlinedIcon />} onClick={() => navigate(-1)} sx={{ textTransform: 'none', fontWeight: 600 }}>
          Go Back
        </Button>
      </Box>
    );
  }

  if (!data) {
    return (
      <Box sx={{ bgcolor: '#F8FAFC', minHeight: '100vh', p: 3 }}>
        <Box sx={{ maxWidth: 480, mx: 'auto', mt: 8, textAlign: 'center' }}>
          <Typography sx={{ fontSize: 20, fontWeight: 700, color: '#111827', mb: 1 }}>Employee Not Found</Typography>
          <Typography sx={{ fontSize: 14, color: '#6B7280', mb: 3 }}>The requested employee does not exist or has been removed.</Typography>
          <Button startIcon={<ArrowBackOutlinedIcon />} onClick={() => navigate('/employees')} sx={{ textTransform: 'none', fontWeight: 600 }}>
            Back to Employees
          </Button>
        </Box>
      </Box>
    );
  }

  return (
    <Box sx={{ bgcolor: '#F8FAFC', minHeight: '100vh' }}>
      <Box sx={{ maxWidth: 1600, mx: 'auto', px: '24px', pt: '16px', pb: '24px' }}>
        {/* Back */}
        <Button
          startIcon={<ArrowBackOutlinedIcon />}
          onClick={() => navigate('/employees')}
          size="small"
          sx={{ textTransform: 'none', fontWeight: 600, fontSize: 12, color: '#6B7280', p: 0, minWidth: 0, mb: 1.5, '&:hover': { color: '#2563EB', bgcolor: 'transparent' } }}
        >
          Back to Employees
        </Button>
        <Typography sx={{ fontSize: 32, fontWeight: 700, color: '#111827', lineHeight: 1.15, mb: 2 }}>
          Employee Profile
        </Typography>

        {/* Header */}
        <ProfileHeader data={data} totalAllocated={totalAllocated} />

        {/* Tabs */}
        <Box sx={{ mt: 2, borderBottom: '1px solid #E5E7EB', bgcolor: '#FFF', borderRadius: '12px 12px 0 0', border: '1px solid #E5E7EB', borderBottom: 'none' }}>
          <Tabs
            value={tabValue}
            onChange={handleTabChange}
            variant="scrollable"
            scrollButtons="auto"
            sx={{
              minHeight: 48,
              px: 1.5,
              '& .MuiTab-root': {
                textTransform: 'none',
                fontWeight: 600,
                fontSize: '0.8125rem',
                minHeight: 48,
                px: 2.5,
                py: 0,
                color: '#6B7280',
                borderRadius: '8px 8px 0 0',
                transition: 'all 200ms ease',
                '&.Mui-selected': {
                  color: '#2563EB',
                  bgcolor: '#EFF6FF',
                  fontWeight: 700,
                },
                '&:hover:not(.Mui-selected)': {
                  bgcolor: '#F8FAFC',
                  color: '#374151',
                },
              },
              '& .MuiTabs-indicator': {
                height: 3,
                borderRadius: '3px 3px 0 0',
                bgcolor: '#2563EB',
              },
            }}
          >
            {tabs.map((tab) => (
              <Tab key={tab} label={tab} />
            ))}
          </Tabs>
        </Box>

        {/* ──────────────────────────────────────────────── */}
        {/* TAB 0: EMPLOYEE INFORMATION                      */}
        {/* ──────────────────────────────────────────────── */}
        {tabValue === 0 && (
          <Box sx={{ mt: '16px', display: 'flex', flexDirection: 'column', gap: '16px' }}>
            {/* Personal Information */}
            <InfoSection {...sectionConfig.personal}>
              <InfoGrid>
                <InfoField icon={<BadgeOutlinedIcon sx={{ fontSize: 16 }} />} label="Employee Code" value={data.employeeCode} />
                <InfoField icon={<PersonOutlineOutlinedIcon sx={{ fontSize: 16 }} />} label="Full Name" value={data.employeeName} />
                <InfoField icon={<EmailOutlinedIcon sx={{ fontSize: 16 }} />} label="Email" value={data.email} />
                <InfoField icon={<PhoneOutlinedIcon sx={{ fontSize: 16 }} />} label="Mobile Number" value="—" />
                <InfoField icon={<WcOutlinedIcon sx={{ fontSize: 16 }} />} label="Gender" value="—" />
                <InfoField icon={<CalendarTodayOutlinedIcon sx={{ fontSize: 16 }} />} label="Date of Birth" value="—" />
                <InfoField icon={<BloodtypeOutlinedIcon sx={{ fontSize: 16 }} />} label="Blood Group" value="—" />
                <InfoField icon={<WcOutlinedIcon sx={{ fontSize: 16 }} />} label="Marital Status" value="—" />
              </InfoGrid>
            </InfoSection>

            {/* Organization Information */}
            <InfoSection {...sectionConfig.organization}>
              <InfoGrid>
                <InfoField icon={<BadgeOutlinedIcon sx={{ fontSize: 16 }} />} label="Designation" value={data.role ?? '—'} />
                <InfoField icon={<AccountBalanceOutlinedIcon sx={{ fontSize: 16 }} />} label="Department" value={data.practice ?? '—'} />
                <InfoField icon={<AccountBalanceOutlinedIcon sx={{ fontSize: 16 }} />} label="Business Unit" value={data.practice ?? '—'} />
                <InfoField icon={<WorkOutlineOutlinedIcon sx={{ fontSize: 16 }} />} label="Office" value={data.location ?? '—'} />
                <InfoField icon={<PsychologyOutlinedIcon sx={{ fontSize: 16 }} />} label="Primary Skill" value={data.primarySkill ?? '—'} />
                <InfoField icon={<PsychologyOutlinedIcon sx={{ fontSize: 16 }} />} label="Secondary Skill" value={data.skill ?? '—'} colSpan={2} />
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

        {/* ──────────────────────────────────────────────── */}
        {/* TAB 1: EXPERIENCE                                */}
        {/* ──────────────────────────────────────────────── */}
        {tabValue === 1 && (
          <Box sx={{ mt: '16px' }}>
            <InfoSection {...sectionConfig.experience}>
              <InfoGrid>
                <InfoField icon={<WorkspacePremiumOutlinedIcon sx={{ fontSize: 16 }} />} label="Total Experience" value={`${data.totalExperience} yrs`} />
                <InfoField icon={<WorkspacePremiumOutlinedIcon sx={{ fontSize: 16 }} />} label="NV Experience" value={`${data.nvExperience} yrs`} />
                <InfoField icon={<WorkspacePremiumOutlinedIcon sx={{ fontSize: 16 }} />} label="Prior Experience" value={`${data.priorExperience} yrs`} />
                <InfoField icon={<WorkspacePremiumOutlinedIcon sx={{ fontSize: 16 }} />} label="Experience Range" value={data.experienceRange} />
                <InfoField icon={<PsychologyOutlinedIcon sx={{ fontSize: 16 }} />} label="Primary Skill" value={data.primarySkill ?? '—'} />
                <InfoField icon={<PsychologyOutlinedIcon sx={{ fontSize: 16 }} />} label="Secondary Skills" value={data.skill ?? '—'} colSpan={3} />
              </InfoGrid>
            </InfoSection>
          </Box>
        )}

        {/* ──────────────────────────────────────────────── */}
        {/* TAB 2: EMPLOYMENT                                */}
        {/* ──────────────────────────────────────────────── */}
        {tabValue === 2 && (
          <Box sx={{ mt: '16px' }}>
            <InfoSection {...sectionConfig.employment}>
              <InfoGrid>
                <InfoField icon={<AssignmentOutlinedIcon sx={{ fontSize: 16 }} />} label="Employment Status" value={data.status ?? '—'} />
                <InfoField icon={<AssignmentOutlinedIcon sx={{ fontSize: 16 }} />} label="Employee Type" value={data.fteConsultant ?? '—'} />
                <InfoField icon={<WorkOutlineOutlinedIcon sx={{ fontSize: 16 }} />} label="Work Model" value="—" />
                <InfoField icon={<HistoryEduOutlinedIcon sx={{ fontSize: 16 }} />} label="Notice Period" value="—" />
                <InfoField icon={<CalendarTodayOutlinedIcon sx={{ fontSize: 16 }} />} label="Joining Date" value={formatDate(data.doj)} />
                <InfoField icon={<CalendarTodayOutlinedIcon sx={{ fontSize: 16 }} />} label="Last Working Day" value="—" />
                <InfoField icon={<CalendarTodayOutlinedIcon sx={{ fontSize: 16 }} />} label="Billing Status" value={data.billable ?? '—'} />
                <InfoField icon={<AccountBalanceOutlinedIcon sx={{ fontSize: 16 }} />} label="Department" value={data.practice ?? '—'} />
              </InfoGrid>
            </InfoSection>
          </Box>
        )}

        {/* ──────────────────────────────────────────────── */}
        {/* TAB 3: ALLOCATION HISTORY                        */}
        {/* ──────────────────────────────────────────────── */}
        {tabValue === 3 && (
          <Box sx={{ mt: '16px' }}>
            <InfoSection {...sectionConfig.allocation}>
              <InfoGrid>
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Total Allocation" value={`${totalAllocated}%`} />
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Available Capacity" value={`${Math.max(0, 100 - totalAllocated)}%`} />
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Active Projects" value={data.projectAllocations.length} />
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Utilisation" value={totalAllocated > 100 ? 'Overallocated' : totalAllocated >= 80 ? 'High' : 'Normal'} />
              </InfoGrid>
            </InfoSection>
          </Box>
        )}

        {/* ──────────────────────────────────────────────── */}
        {/* TAB 4: DOCUMENTS                                 */}
        {/* ──────────────────────────────────────────────── */}
        {tabValue === 4 && (
          <Box sx={{ mt: '16px' }}>
            <Box sx={{ borderRadius: '12px', border: '1px solid #E5E7EB', bgcolor: '#FFF', p: 6, textAlign: 'center' }}>
              <Typography sx={{ fontSize: 16, fontWeight: 600, color: '#6B7280', mb: 1 }}>Documents</Typography>
              <Typography sx={{ fontSize: 14, color: '#9CA3AF' }}>No documents available for this employee.</Typography>
            </Box>
          </Box>
        )}
      </Box>
    </Box>
  );
}
