import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Alert,
  Box,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tab,
  Tabs,
  Typography,
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { useState } from 'react';
import type { BulkUploadPreview, EmployeeChange } from '../services/employeeService';

interface Props {
  open: boolean;
  preview: BulkUploadPreview;
  onConfirm: () => void;
  onCancel: () => void;
}

function ChangesAccordion({ change }: { change: EmployeeChange }) {
  return (
    <Accordion>
      <AccordionSummary expandIcon={<ExpandMoreIcon />}>
        <Stack direction="row" spacing={1} alignItems="center">
          <Typography fontWeight={600}>{change.fullName}</Typography>
          <Typography variant="body2" color="text.secondary" sx={{ display: { xs: 'none', sm: 'block' } }}>
            ({change.email})
          </Typography>
          <Chip label={`${change.fieldChanges.length} change${change.fieldChanges.length > 1 ? 's' : ''}`} size="small" color="warning" />
        </Stack>
      </AccordionSummary>
      <AccordionDetails>
        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell sx={{ fontWeight: 600 }}>Field</TableCell>
                <TableCell sx={{ fontWeight: 600 }}>Old Value</TableCell>
                <TableCell sx={{ fontWeight: 600 }}>New Value</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {change.fieldChanges.map((fc, i) => (
                <TableRow key={i}>
                  <TableCell>{fc.fieldName}</TableCell>
                  <TableCell>
                    <Typography variant="body2" sx={{ textDecoration: 'line-through', color: 'error.main' }}>
                      {fc.oldValue ?? '-'}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" sx={{ fontWeight: 600, color: 'success.main' }}>
                      {fc.newValue ?? '-'}
                    </Typography>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </AccordionDetails>
    </Accordion>
  );
}

export default function BulkUploadPreviewDialog({ open, preview, onConfirm, onCancel }: Props) {
  const [tab, setTab] = useState(0);

  const hasChanges = preview.updatedEmployees > 0 || preview.newEmployees > 0 || preview.deletedEmployees > 0;

  return (
    <Dialog fullWidth maxWidth="md" open={open} onClose={onCancel}>
      <DialogTitle>Review Changes Before Import</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          {!preview.success ? (
            <Alert severity="error">{preview.errorMessage ?? 'Preview failed.'}</Alert>
          ) : !hasChanges ? (
            <Alert severity="info">No changes detected. All {preview.totalRows} rows match existing data.</Alert>
          ) : (
            <>
              <Alert severity={preview.deletedEmployees > 0 ? 'warning' : 'info'}>
                <Typography variant="body2">
                  <strong>{preview.newEmployees}</strong> new employee{preview.newEmployees !== 1 ? 's' : ''},
                  <strong> {preview.updatedEmployees}</strong> employee{preview.updatedEmployees !== 1 ? 's' : ''} with changes,
                  <strong> {preview.deletedEmployees}</strong> employee{preview.deletedEmployees !== 1 ? 's' : ''} to be removed
                  <Typography variant="caption" color="text.secondary" sx={{ ml: 1 }}>
                    (across {preview.totalRows} total row{preview.totalRows !== 1 ? 's' : ''})
                  </Typography>
                </Typography>
              </Alert>

              <Tabs onChange={(_, v) => setTab(v)} value={tab} variant="scrollable">
                {preview.updatedEmployees > 0 && <Tab label={`Updated (${preview.updatedEmployees})`} />}
                {preview.newEmployees > 0 && <Tab label={`New (${preview.newEmployees})`} />}
                {preview.deletedEmployees > 0 && <Tab label={`Removed (${preview.deletedEmployees})`} />}
              </Tabs>

              {tab === 0 && preview.updatedEmployees > 0 && (
                <Stack spacing={1}>
                  {preview.changes.map((c, i) => (
                    <ChangesAccordion key={i} change={c} />
                  ))}
                </Stack>
              )}

              {(tab === (preview.updatedEmployees > 0 ? 1 : 0)) && preview.newEmployees > 0 && (
                <Box>
                  {preview.newEmployeeList.map((e, i) => (
                    <Chip key={i} label={`${e.fullName} (${e.email})`} sx={{ m: 0.3 }} variant="outlined" color="info" />
                  ))}
                </Box>
              )}

              {(tab === (preview.updatedEmployees > 0 && preview.newEmployees > 0 ? 2 : preview.updatedEmployees > 0 ? 1 : 0)) && preview.deletedEmployees > 0 && (
                <Alert severity="warning">
                  <Typography variant="body2" fontWeight={600} gutterBottom>
                    These employees will be marked as inactive (soft-deleted):
                  </Typography>
                  <Box sx={{ mt: 1 }}>
                    {preview.deletedEmployeeList.map((e, i) => (
                      <Chip key={i} label={`${e.fullName} (${e.email})`} sx={{ m: 0.3 }} variant="outlined" color="error" />
                    ))}
                  </Box>
                </Alert>
              )}
            </>
          )}
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel}>Cancel</Button>
        {preview.success && hasChanges && (
          <Button color="primary" onClick={onConfirm} variant="contained">
            Proceed with Import
          </Button>
        )}
      </DialogActions>
    </Dialog>
  );
}
