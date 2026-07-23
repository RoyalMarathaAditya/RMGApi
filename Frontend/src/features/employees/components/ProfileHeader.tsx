import { useMemo } from 'react';
import { Avatar, Box, Chip, Typography, useTheme } from '@mui/material';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import AssessmentOutlinedIcon from '@mui/icons-material/AssessmentOutlined';
import FolderOutlinedIcon from '@mui/icons-material/FolderOutlined';
import CalendarTodayOutlinedIcon from '@mui/icons-material/CalendarTodayOutlined';
import PersonOutlineOutlinedIcon from '@mui/icons-material/PersonOutlineOutlined';
import type { EmployeeResourceDetailsDto } from '../../rmg/types/allocation';

interface ProfileHeaderProps {
  data: EmployeeResourceDetailsDto;
  totalAllocated: number;
}

function formatDate(dateStr: string | null | undefined): string {
  if (!dateStr) return '—';
  const d = new Date(dateStr);
  return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
}

function getExperienceFromDoj(doj: string, lwd?: string | null): { years: number; months: number } {
  const from = new Date(doj);
  const to = lwd ? new Date(lwd) : new Date();
  if (to < from) return { years: 0, months: 0 };
  let years = to.getFullYear() - from.getFullYear();
  let months = to.getMonth() - from.getMonth();
  if (months < 0) { years--; months += 12; }
  return { years, months };
}

function getExperienceRange(totalYears: number): string {
  if (totalYears < 2) return '0-2 Years';
  if (totalYears < 5) return '2-5 Years';
  if (totalYears < 8) return '5-8 Years';
  if (totalYears < 12) return '8-12 Years';
  if (totalYears < 15) return '12-15 Years';
  return '15+ Years';
}

export default function ProfileHeader({ data, totalAllocated }: ProfileHeaderProps) {
  const theme = useTheme();
  const isDark = theme.palette.mode === 'dark';

  const darkBg = (light: string, dark: string) => isDark ? dark : light;

  const nvExperience = useMemo(() => {
    if (!data.doj) return null;
    return getExperienceFromDoj(data.doj, data.lwd);
  }, [data.doj, data.lwd]);

  const totalExperienceDisplay = useMemo(() => {
    if (!nvExperience) return '—';
    const priorYears = Math.floor(data.priorExperience ?? 0);
    const priorMonths = Math.round(((data.priorExperience ?? 0) - priorYears) * 12);
    let totalMonths = nvExperience.months + priorMonths;
    let totalYears = nvExperience.years + priorYears;
    if (totalMonths >= 12) { totalYears++; totalMonths -= 12; }
    return `${totalYears}.${totalMonths} Years`;
  }, [nvExperience, data.priorExperience]);

  const experienceRange = useMemo(() => {
    if (!nvExperience) return '—';
    const priorYears = Math.floor(data.priorExperience ?? 0);
    const priorMonths = Math.round(((data.priorExperience ?? 0) - priorYears) * 12);
    let totalMonths = nvExperience.months + priorMonths;
    let totalYears = nvExperience.years + priorYears;
    if (totalMonths >= 12) { totalYears++; }
    return getExperienceRange(totalYears);
  }, [nvExperience, data.priorExperience]);

  const kpiItems = [
    { icon: <LocationOnOutlinedIcon sx={{ fontSize: 14 }} />, label: 'Location', value: data.location ?? '—', bg: darkBg('#EFF6FF', '#1A3A5C'), iconColor: '#2563EB' },
    {
      icon: <AssessmentOutlinedIcon sx={{ fontSize: 14 }} />,
      label: 'Allocation',
      value: `${totalAllocated}%`,
      color: totalAllocated > 100 ? '#DC2626' : totalAllocated >= 80 ? '#F59E0B' : '#16A34A',
      bg: darkBg(
        totalAllocated > 100 ? '#FEE2E2' : totalAllocated >= 80 ? '#FEF3C7' : '#DCFCE7',
        totalAllocated > 100 ? '#5C1A1A' : totalAllocated >= 80 ? '#5C4A1A' : '#1A5C1A',
      ),
      iconColor: totalAllocated > 100 ? '#DC2626' : totalAllocated >= 80 ? '#F59E0B' : '#16A34A',
    },
    { icon: <FolderOutlinedIcon sx={{ fontSize: 14 }} />, label: 'Projects', value: data.projectAllocations.length, bg: darkBg('#F5F3FF', '#3A2A5C'), iconColor: '#7C3AED' },
    { icon: <CalendarTodayOutlinedIcon sx={{ fontSize: 14 }} />, label: 'Joined', value: formatDate(data.doj), bg: darkBg('#FFF7ED', '#5C3A1A'), iconColor: '#D97706' },
    { icon: <PersonOutlineOutlinedIcon sx={{ fontSize: 14 }} />, label: 'Manager', value: data.l1Manager ?? '—', bg: darkBg('#ECFDF5', '#1A4A3A'), iconColor: '#059669' },
  ];

  return (
    <Box
      sx={{
        display: 'flex',
        alignItems: 'center',
        gap: 2,
        height: 152,
        px: 3,
        bgcolor: theme.palette.background.paper,
        borderRadius: '12px',
        border: `1px solid ${theme.palette.divider}`,
        boxShadow: '0 1px 3px rgba(0,0,0,.04)',
      }}
    >
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, flexShrink: 0 }}>
        <Avatar
          sx={{
            width: 56,
            height: 56,
            bgcolor: theme.palette.primary.main,
            fontSize: '1.3rem',
            fontWeight: 700,
            color: theme.palette.primary.contrastText,
            boxShadow: `0 2px 8px ${theme.palette.primary.main}33`,
          }}
        >
          {data.employeeName?.charAt(0)?.toUpperCase() ?? '?'}
        </Avatar>
        <Box>
          <Typography sx={{ fontSize: 18, fontWeight: 700, color: theme.palette.text.primary, lineHeight: 1.2 }}>
            {data.employeeName}
          </Typography>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.75, mt: 0.25 }}>
            <Typography sx={{ fontSize: 12, fontWeight: 600, color: theme.palette.primary.main, fontFamily: 'monospace' }}>
              {data.employeeCode}
            </Typography>
            <Typography sx={{ fontSize: 11, color: theme.palette.text.disabled }}>|</Typography>
            <Typography sx={{ fontSize: 12, fontWeight: 500, color: theme.palette.text.secondary }}>
              {data.role ?? '—'}
            </Typography>
            <Typography sx={{ fontSize: 11, color: theme.palette.text.disabled }}>|</Typography>
            <Typography sx={{ fontSize: 12, fontWeight: 500, color: theme.palette.text.secondary }}>
              {data.practice ?? '—'}
            </Typography>
          </Box>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.75, mt: 0.75 }}>
            <Chip
              label={data.status ?? '—'}
              size="small"
              sx={{
                height: 20, fontSize: '0.65rem', fontWeight: 700,
                bgcolor: darkBg('#EFF6FF', '#1A3A5C'),
                color: darkBg('#2563EB', '#60A5FA'),
                borderRadius: '999px',
              }}
            />
            <Chip
              label={totalExperienceDisplay}
              size="small"
              variant="outlined"
              sx={{ height: 20, fontSize: '0.65rem', fontWeight: 600, borderColor: theme.palette.divider, color: theme.palette.text.secondary, borderRadius: '999px' }}
            />
            <Chip
              label={experienceRange}
              size="small"
              variant="outlined"
              sx={{ height: 20, fontSize: '0.65rem', fontWeight: 600, borderColor: theme.palette.divider, color: theme.palette.text.secondary, borderRadius: '999px' }}
            />
          </Box>
        </Box>
      </Box>

      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, ml: 'auto', flexShrink: 0 }}>
        {kpiItems.map((kpi) => (
          <Box
            key={kpi.label}
            sx={{
              width: 136,
              height: 60,
              borderRadius: '10px',
              bgcolor: (kpi as any).bg,
              border: `1px solid ${theme.palette.divider}`,
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'center',
              px: 1.25,
              transition: 'all 200ms ease',
              '&:hover': { transform: 'translateY(-1px)', boxShadow: '0 2px 8px rgba(0,0,0,.06)' },
            }}
          >
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, mb: 0.25 }}>
              <Box sx={{ color: (kpi as any).iconColor ?? theme.palette.text.secondary, display: 'flex', lineHeight: 0 }}>{kpi.icon}</Box>
              <Typography
                component="span"
                sx={{ fontSize: 9, fontWeight: 600, color: theme.palette.text.secondary, textTransform: 'uppercase', letterSpacing: '0.4px', lineHeight: 1 }}
              >
                {kpi.label}
              </Typography>
            </Box>
            <Typography
              sx={{
                fontSize: 16,
                fontWeight: 700,
                color: 'color' in kpi ? (kpi as any).color : theme.palette.text.primary,
                lineHeight: 1.2,
                textOverflow: 'ellipsis',
                overflow: 'hidden',
                whiteSpace: 'nowrap',
              }}
            >
              {kpi.value}
            </Typography>
          </Box>
        ))}
      </Box>
    </Box>
  );
}
