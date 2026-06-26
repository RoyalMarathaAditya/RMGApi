import {
  Box,
  Chip,
  Drawer,
  IconButton,
  Typography,
  Divider,
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import CalendarTodayOutlinedIcon from '@mui/icons-material/CalendarTodayOutlined';
import AccountBalanceOutlinedIcon from '@mui/icons-material/AccountBalanceOutlined';
import type { ProjectAllocationDetailDto } from '../types/allocation';

interface ProjectDrawerProps {
  open: boolean;
  onClose: () => void;
  project: ProjectAllocationDetailDto | null;
}

function formatDate(dateStr: string | null | undefined): string {
  if (!dateStr) return '—';
  const d = new Date(dateStr);
  return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
}

const statusColors: Record<string, { bg: string; text: string }> = {
  Billable: { bg: '#DCFCE7', text: '#15803D' },
  'Non-Billable': { bg: '#FEF3C7', text: '#B45309' },
  Shadow: { bg: '#E0F2FE', text: '#0369A1' },
  Active: { bg: '#DCFCE7', text: '#15803D' },
  Planned: { bg: '#E0F2FE', text: '#0369A1' },
  Completed: { bg: '#F3F4F6', text: '#374151' },
  Released: { bg: '#FEF3C7', text: '#B45309' },
  Cancelled: { bg: '#FEE2E2', text: '#B91C1C' },
};

export default function ProjectDrawer({ open, onClose, project }: ProjectDrawerProps) {
  if (!project) return null;

  const statusStyle = statusColors[project.projectStatus ?? ''] ?? { bg: '#F3F4F6', text: '#374151' };

  return (
    <Drawer
      anchor="right"
      open={open}
      onClose={onClose}
      PaperProps={{
        sx: {
          width: 480,
          p: 2.5,
          borderRadius: '16px 0 0 16px',
          borderLeft: '1px solid #E5E7EB',
        },
      }}
    >
      {/* Header */}
      <Box sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', mb: 1.5 }}>
        <Box>
          <Typography sx={{ fontSize: 18, fontWeight: 700, color: '#111827', lineHeight: 1.2 }}>
            {project.project ?? 'Project Details'}
          </Typography>
          <Typography sx={{ fontSize: 11, fontWeight: 500, color: '#6B7280', mt: 0.25 }}>
            {project.client ? `${project.client} • ` : ''}Code: {project.projectCode ?? '—'}
          </Typography>
        </Box>
        <IconButton onClick={onClose} size="small" sx={{ color: '#6B7280' }}>
          <CloseIcon fontSize="small" />
        </IconButton>
      </Box>

      <Divider sx={{ mb: 1.5 }} />

      {/* Status badge */}
      <Box sx={{ display: 'flex', gap: 1, mb: 2.5 }}>
        <Chip
          label={project.projectStatus ?? '—'}
          size="small"
          sx={{ height: 22, fontSize: '0.7rem', fontWeight: 600, bgcolor: statusStyle.bg, color: statusStyle.text, borderRadius: '999px' }}
        />
        {project.projectType && (
          <Chip
            label={project.projectType}
            size="small"
            variant="outlined"
            sx={{ height: 22, fontSize: '0.7rem', fontWeight: 600, borderColor: '#D1D5DB', borderRadius: '999px' }}
          />
        )}
      </Box>

      {/* Project Information */}
      <Typography sx={{ fontSize: 12, fontWeight: 600, color: '#374151', textTransform: 'uppercase', letterSpacing: '0.4px', mb: 1.25 }}>
        Project Information
      </Typography>

      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.25, mb: 2.5 }}>
        <Row label="Client" value={project.client} />
        <Row label="Project Code" value={String(project.projectCode ?? '—')} />
        <Row label="Project Type" value={project.projectType} />
        <Row label="Status" value={<Chip label={project.projectStatus ?? '—'} size="small" sx={{ height: 20, fontSize: '0.65rem', fontWeight: 600, bgcolor: statusStyle.bg, color: statusStyle.text, borderRadius: '999px' }} />} />
      </Box>

      <Divider sx={{ mb: 2.5 }} />

      {/* Allocation Timeline */}
      <Typography sx={{ fontSize: 12, fontWeight: 600, color: '#374151', textTransform: 'uppercase', letterSpacing: '0.4px', mb: 1.25 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
          <CalendarTodayOutlinedIcon sx={{ fontSize: 13 }} />
          Allocation Timeline
        </Box>
      </Typography>

      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.25, mb: 2.5 }}>
        <Row label="Start Date" value={formatDate(project.startDate)} />
        <Row label="End Date" value={formatDate(project.endDate)} />
        <Row label="Duration" value={project.durationInProject ?? '—'} />
        <Row label="Allocation" value={project.allocationPercentage != null ? `${project.allocationPercentage}%` : '—'} />
        <Row label="Billable" value={project.billablePercentage != null ? `${project.billablePercentage}%` : '—'} />
      </Box>

      <Divider sx={{ mb: 2.5 }} />

      {/* Additional Info */}
      <Typography sx={{ fontSize: 12, fontWeight: 600, color: '#374151', textTransform: 'uppercase', letterSpacing: '0.4px', mb: 1.25 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
          <AccountBalanceOutlinedIcon sx={{ fontSize: 13 }} />
          Additional Information
        </Box>
      </Typography>

      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.25 }}>
        <Row label="Ageing" value={project.ageing ?? '—'} />
        <Row label="Engineering" value={project.engineering ?? '—'} />
        <Row label="Remarks" value={project.remarks ?? '—'} />
      </Box>
    </Drawer>
  );
}

function Row({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
      <Typography sx={{ fontSize: 12, fontWeight: 500, color: '#6B7280' }}>{label}</Typography>
      <Typography sx={{ fontSize: 13, fontWeight: 600, color: '#111827', textAlign: 'right', maxWidth: '60%', wordBreak: 'break-word' }}>
        {value ?? '—'}
      </Typography>
    </Box>
  );
}
