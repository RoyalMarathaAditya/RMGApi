import { Avatar, Box, Chip, Typography } from '@mui/material';
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

export default function ProfileHeader({ data, totalAllocated }: ProfileHeaderProps) {
  const kpiItems = [
    { icon: <LocationOnOutlinedIcon sx={{ fontSize: 14 }} />, label: 'Location', value: data.location ?? '—', bg: '#EFF6FF', iconColor: '#2563EB' },
    {
      icon: <AssessmentOutlinedIcon sx={{ fontSize: 14 }} />,
      label: 'Allocation',
      value: `${totalAllocated}%`,
      color: totalAllocated > 100 ? '#DC2626' : totalAllocated >= 80 ? '#F59E0B' : '#16A34A',
      bg: totalAllocated > 100 ? '#FEE2E2' : totalAllocated >= 80 ? '#FEF3C7' : '#DCFCE7',
      iconColor: totalAllocated > 100 ? '#DC2626' : totalAllocated >= 80 ? '#F59E0B' : '#16A34A',
    },
    { icon: <FolderOutlinedIcon sx={{ fontSize: 14 }} />, label: 'Projects', value: data.projectAllocations.length, bg: '#F5F3FF', iconColor: '#7C3AED' },
    { icon: <CalendarTodayOutlinedIcon sx={{ fontSize: 14 }} />, label: 'Joined', value: formatDate(data.doj), bg: '#FFF7ED', iconColor: '#D97706' },
    { icon: <PersonOutlineOutlinedIcon sx={{ fontSize: 14 }} />, label: 'Manager', value: data.l1Manager ?? '—', bg: '#ECFDF5', iconColor: '#059669' },
  ];

  return (
    <Box
      sx={{
        display: 'flex',
        alignItems: 'center',
        gap: 2,
        height: 152,
        px: 3,
        bgcolor: '#FFF',
        borderRadius: '12px',
        border: '1px solid #E5E7EB',
        boxShadow: '0 1px 3px rgba(0,0,0,.04)',
      }}
    >
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, flexShrink: 0 }}>
        <Avatar
          sx={{
            width: 56,
            height: 56,
            bgcolor: '#2563EB',
            fontSize: '1.3rem',
            fontWeight: 700,
            color: '#FFF',
            boxShadow: '0 2px 8px rgba(37,99,235,.2)',
          }}
        >
          {data.employeeName?.charAt(0)?.toUpperCase() ?? '?'}
        </Avatar>
        <Box>
          <Typography sx={{ fontSize: 18, fontWeight: 700, color: '#111827', lineHeight: 1.2 }}>
            {data.employeeName}
          </Typography>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.75, mt: 0.25 }}>
            <Typography sx={{ fontSize: 12, fontWeight: 600, color: '#2563EB', fontFamily: 'monospace' }}>
              {data.employeeCode}
            </Typography>
            <Typography sx={{ fontSize: 11, color: '#9CA3AF' }}>|</Typography>
            <Typography sx={{ fontSize: 12, fontWeight: 500, color: '#374151' }}>
              {data.role ?? '—'}
            </Typography>
            <Typography sx={{ fontSize: 11, color: '#9CA3AF' }}>|</Typography>
            <Typography sx={{ fontSize: 12, fontWeight: 500, color: '#374151' }}>
              {data.practice ?? '—'}
            </Typography>
          </Box>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.75, mt: 0.75 }}>
            <Chip
              label={data.active ? 'Active' : 'Inactive'}
              size="small"
              sx={{
                height: 20, fontSize: '0.65rem', fontWeight: 700,
                bgcolor: data.active ? '#DCFCE7' : '#FEE2E2',
                color: data.active ? '#15803D' : '#B91C1C',
                borderRadius: '999px',
              }}
            />
            <Chip
              label={`${data.totalExperience} yrs`}
              size="small"
              variant="outlined"
              sx={{ height: 20, fontSize: '0.65rem', fontWeight: 600, borderColor: '#D1D5DB', color: '#374151', borderRadius: '999px' }}
            />
            <Chip
              label={data.experienceRange}
              size="small"
              variant="outlined"
              sx={{ height: 20, fontSize: '0.65rem', fontWeight: 600, borderColor: '#D1D5DB', color: '#374151', borderRadius: '999px' }}
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
              border: '1px solid #EEF2F7',
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'center',
              px: 1.25,
              transition: 'all 200ms ease',
              '&:hover': { transform: 'translateY(-1px)', boxShadow: '0 2px 8px rgba(0,0,0,.06)' },
            }}
          >
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, mb: 0.25 }}>
              <Box sx={{ color: (kpi as any).iconColor ?? '#6B7280', display: 'flex', lineHeight: 0 }}>{kpi.icon}</Box>
              <Typography
                component="span"
                sx={{ fontSize: 9, fontWeight: 600, color: '#6B7280', textTransform: 'uppercase', letterSpacing: '0.4px', lineHeight: 1 }}
              >
                {kpi.label}
              </Typography>
            </Box>
            <Typography
              sx={{
                fontSize: 16,
                fontWeight: 700,
                color: 'color' in kpi ? (kpi as any).color : '#111827',
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
