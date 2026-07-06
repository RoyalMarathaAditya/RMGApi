import {
  Box,
  Chip,
  Drawer,
  IconButton,
  Typography,
  Divider,
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
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
          width: 500,
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

      {/* Status badges */}
      <Box sx={{ display: 'flex', gap: 1, mb: 2.5, flexWrap: 'wrap' }}>
        <Chip
          label={project.projectStatus ?? '—'}
          size="small"
          sx={{ height: 22, fontSize: '0.7rem', fontWeight: 600, bgcolor: statusStyle.bg, color: statusStyle.text, borderRadius: '999px' }}
        />
        {project.allocationStatus && (
          <Chip
            label={project.allocationStatus}
            size="small"
            variant="outlined"
            sx={{ height: 22, fontSize: '0.7rem', fontWeight: 600, borderColor: '#D1D5DB', borderRadius: '999px' }}
          />
        )}
      </Box>

      {/* ── Section 1: Project Information ── */}
      <Typography sx={{ fontSize: 12, fontWeight: 600, color: '#374151', textTransform: 'uppercase', letterSpacing: '0.4px', mb: 1.25 }}>
        Project Information
      </Typography>
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.25, mb: 2.5 }}>
        <Row label="Project Name" value={project.project} />
        <Row label="Project Code" value={project.projectCode} />
        <Row label="Client" value={project.client} />
        <Row label="Project Manager" value={project.projectManager} />
        <Row label="Delivery Head" value={project.deliveryHead} />
        <Row label="CSM" value={project.csm} />
        <Row label="Project Status" value={project.projectStatus} />
        <Row label="Allocation Status" value={project.allocationStatus} />
      </Box>

      <Divider sx={{ mb: 2.5 }} />

      {/* ── Section 2: Allocation Information ── */}
      <Typography sx={{ fontSize: 12, fontWeight: 600, color: '#374151', textTransform: 'uppercase', letterSpacing: '0.4px', mb: 1.25 }}>
        Allocation Information
      </Typography>
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.25, mb: 2.5 }}>
        <Row label="Allocation %" value={project.allocationPercentage != null ? `${project.allocationPercentage}%` : null} />
        <Row label="Allocation Type" value={project.allocationType} />
        <Row label="Status" value={project.status} />
        <Row label="Billable Status" value={project.billableStatus} />
        <Row label="Current Billing Status" value={project.currentBillingStatus} />
        <Row label="Billable Date Probability" value={project.billableDateProbability} />
        <Row label="Billing Bucket" value={project.billingBucket} />
      </Box>

      <Divider sx={{ mb: 2.5 }} />

      {/* ── Section 3: Timeline ── */}
      <Typography sx={{ fontSize: 12, fontWeight: 600, color: '#374151', textTransform: 'uppercase', letterSpacing: '0.4px', mb: 1.25 }}>
        Timeline
      </Typography>
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.25, mb: 2.5 }}>
        <Row label="Start Date" value={formatDate(project.startDate)} />
        <Row label="End Date" value={formatDate(project.endDate)} />
        <Row label="Duration" value={project.durationInProject} />
        <Row label="Ageing" value={project.ageing} />
        <Row label="Ageing Bucket" value={project.ageingBucket} />
        <Row label="Probable Next Assignment" value={project.probableNextAssignment} />
        <Row label="Probable Next Assignment Date" value={formatDate(project.probableNextAssignmentDate)} />
      </Box>

      <Divider sx={{ mb: 2.5 }} />

      {/* ── Section 4: Additional Information ── */}
      <Typography sx={{ fontSize: 12, fontWeight: 600, color: '#374151', textTransform: 'uppercase', letterSpacing: '0.4px', mb: 1.25 }}>
        Additional Information
      </Typography>
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.25, mb: 2.5 }}>
        <Row label="Engineering" value={project.engineering} />
      </Box>

      {project.actionItem ? (
        <Box sx={{ mb: 2 }}>
          <Typography sx={{ fontSize: 12, fontWeight: 500, color: '#6B7280', mb: 0.5 }}>Action Item</Typography>
          <Box sx={{ border: '1px solid #E5E7EB', borderRadius: 1.5, p: 1.5, bgcolor: '#F9FAFB', minHeight: 36 }}>
            <Typography sx={{ fontSize: 13, fontWeight: 500, color: '#111827', whiteSpace: 'pre-wrap', wordBreak: 'break-word' }}>
              {project.actionItem}
            </Typography>
          </Box>
        </Box>
      ) : (
        <Row label="Action Item" value={null} />
      )}

      {project.remarks ? (
        <Box sx={{ mb: 2 }}>
          <Typography sx={{ fontSize: 12, fontWeight: 500, color: '#6B7280', mb: 0.5 }}>Remarks</Typography>
          <Box sx={{ border: '1px solid #E5E7EB', borderRadius: 1.5, p: 1.5, bgcolor: '#F9FAFB', minHeight: 36 }}>
            <Typography sx={{ fontSize: 13, fontWeight: 500, color: '#111827', whiteSpace: 'pre-wrap', wordBreak: 'break-word' }}>
              {project.remarks}
            </Typography>
          </Box>
        </Box>
      ) : null}
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
