import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import {
  Alert,
  Box,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  MenuItem,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  ToggleButton,
  ToggleButtonGroup,
  Typography,
} from '@mui/material';
import { useCallback, useEffect, useState } from 'react';
import PageContainer from '../../../components/common/PageContainer';
import { toastService } from '../../../services/toastService';
import { columnMappingService } from '../services/columnMappingService';
import type { ColumnMapping, ColumnMappingFormValues } from '../types/columnMapping';

const dataTypes = ['string', 'datetime', 'int', 'decimal', 'bool'];
const entityTypes = ['employee-import', 'resource-allocation'];
const defaultFormValues: ColumnMappingFormValues = {
  sourceColumn: '',
  targetProperty: '',
  targetDisplayName: '',
  dataType: 'string',
  entityType: 'employee-import',
  isRequired: false,
  isActive: true,
  displayOrder: 0,
};

export default function ColumnMappingList() {
  const [mappings, setMappings] = useState<ColumnMapping[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchText, setSearchText] = useState('');
  const [entityTypeFilter, setEntityTypeFilter] = useState<string>('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<ColumnMapping | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<ColumnMapping | null>(null);
  const [formValues, setFormValues] = useState<ColumnMappingFormValues>(defaultFormValues);
  const [formError, setFormError] = useState('');

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      const data = await columnMappingService.getAll();
      setMappings(data);
    } catch {
      setError('Failed to load column mappings');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void fetchData();
  }, [fetchData]);

  const filtered = mappings.filter((m) => {
    const q = searchText.toLowerCase();
    return (
      (!q ||
        m.sourceColumn.toLowerCase().includes(q) ||
        m.targetProperty.toLowerCase().includes(q) ||
        m.targetDisplayName.toLowerCase().includes(q)) &&
      (!entityTypeFilter || m.entityType === entityTypeFilter)
    );
  });

  const handleOpenAdd = (entityType?: string) => {
    setEditing(null);
    setFormValues({ ...defaultFormValues, entityType: entityType ?? 'employee-import' });
    setFormError('');
    setDialogOpen(true);
  };

  const handleOpenEdit = (m: ColumnMapping) => {
    setEditing(m);
    setFormValues({
      sourceColumn: m.sourceColumn,
      targetProperty: m.targetProperty,
      targetDisplayName: m.targetDisplayName,
      dataType: m.dataType,
      entityType: m.entityType,
      isRequired: m.isRequired,
      isActive: m.isActive,
      displayOrder: m.displayOrder,
    });
    setFormError('');
    setDialogOpen(true);
  };

  const handleSave = async () => {
    if (!formValues.sourceColumn.trim() || !formValues.targetProperty.trim()) {
      setFormError('Source Column and Target Property are required');
      return;
    }
    const duplicateSource = mappings.some(m =>
      m.id !== editing?.id &&
      m.entityType === formValues.entityType &&
      m.sourceColumn.toLowerCase() === formValues.sourceColumn.trim().toLowerCase() &&
      m.isActive
    );
    if (duplicateSource) {
      setFormError(`Source column '${formValues.sourceColumn}' already exists in ${formValues.entityType}`);
      return;
    }
    const displayName = formValues.targetDisplayName.trim();
    if (displayName) {
      const duplicateDisplayName = mappings.some(m =>
        m.id !== editing?.id &&
        m.entityType === formValues.entityType &&
        m.targetDisplayName.toLowerCase() === displayName.toLowerCase() &&
        m.isActive
      );
      if (duplicateDisplayName) {
        setFormError(`Target display name '${formValues.targetDisplayName}' already exists in ${formValues.entityType}`);
        return;
      }
    }
    setFormError('');
    try {
      if (editing) {
        await columnMappingService.update(editing.id, formValues);
        toastService.success('Column mapping updated successfully');
      } else {
        await columnMappingService.create(formValues);
        toastService.success('Column mapping created successfully');
      }
      setDialogOpen(false);
      await fetchData();
    } catch (err: any) {
      setFormError(err?.message || 'Failed to save column mapping');
    }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await columnMappingService.delete(deleteTarget.id);
      setDeleteTarget(null);
      toastService.success('Column mapping deleted successfully');
      await fetchData();
    } catch {
      toastService.error('Failed to delete column mapping');
    }
  };

  return (
    <PageContainer title="Column Mappings">
      <Stack spacing={3}>
        <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
          <Box>
            <Typography color="text.secondary" variant="body2">
              {filtered.length} of {mappings.length} mappings
            </Typography>
          </Box>
          <Stack direction="row" spacing={1}>
            <Button onClick={() => handleOpenAdd('employee-import')} startIcon={<AddIcon />} variant="contained" size="small">
              Add Employee Mapping
            </Button>
            <Button onClick={() => handleOpenAdd('resource-allocation')} startIcon={<AddIcon />} variant="outlined" size="small">
              Add RMG Mapping
            </Button>
          </Stack>
        </Stack>

        <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
          <TextField
            fullWidth
            label="Search mappings"
            onChange={(e) => setSearchText(e.target.value)}
            value={searchText}
          />
          <ToggleButtonGroup
            color="primary"
            exclusive
            onChange={(_, val) => setEntityTypeFilter(val ?? '')}
            size="small"
            value={entityTypeFilter}
          >
            <ToggleButton value="">All</ToggleButton>
            <ToggleButton value="employee-import">Employee Import</ToggleButton>
            <ToggleButton value="resource-allocation">Resource Allocation</ToggleButton>
          </ToggleButtonGroup>
        </Stack>

        {error ? <Alert severity="error">{error}</Alert> : null}

        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell sx={{ fontWeight: 700 }}>Source Column</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Target Property</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Display Name</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Type</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Entity Type</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Required</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Order</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Status</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {filtered.map((m) => (
                  <TableRow key={m.id} hover>
                    <TableCell>{m.sourceColumn}</TableCell>
                    <TableCell>{m.targetProperty}</TableCell>
                    <TableCell>{m.targetDisplayName}</TableCell>
                    <TableCell>
                      <Chip label={m.dataType} size="small" />
                    </TableCell>
                    <TableCell>
                      <Chip
                        color={m.entityType === 'employee-import' ? 'info' : 'secondary'}
                        label={m.entityType === 'employee-import' ? 'Employee' : 'RMG'}
                        size="small"
                        variant="outlined"
                      />
                    </TableCell>
                    <TableCell>
                      <Chip
                        color={m.isRequired ? 'warning' : 'default'}
                        label={m.isRequired ? 'Required' : 'Optional'}
                        size="small"
                        variant="outlined"
                      />
                    </TableCell>
                    <TableCell>{m.displayOrder}</TableCell>
                    <TableCell>
                      <Chip
                        color={m.isActive ? 'success' : 'default'}
                        label={m.isActive ? 'Active' : 'Inactive'}
                        size="small"
                        variant="outlined"
                      />
                    </TableCell>
                    <TableCell>
                      <IconButton color="primary" onClick={() => handleOpenEdit(m)} size="small">
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton color="error" onClick={() => setDeleteTarget(m)} size="small">
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
                {filtered.length === 0 && !loading && (
                  <TableRow>
                    <TableCell colSpan={9} align="center">
                      <Typography color="text.secondary" py={3}>
                        No mappings found
                      </Typography>
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
      </Stack>

      <Dialog fullWidth maxWidth="sm" onClose={() => setDialogOpen(false)} open={dialogOpen}>
        <DialogTitle>{editing ? 'Edit Column Mapping' : 'Add Column Mapping'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            {formError && <Alert severity="error">{formError}</Alert>}
            <TextField
              fullWidth
              label="Source Column"
              onChange={(e) => setFormValues({ ...formValues, sourceColumn: e.target.value })}
              required
              value={formValues.sourceColumn}
            />
            <TextField
              disabled={Boolean(editing)}
              fullWidth
              label="Target Property"
              onChange={(e) => setFormValues({ ...formValues, targetProperty: e.target.value })}
              required
              value={formValues.targetProperty}
            />
            <TextField
              fullWidth
              label="Target Display Name"
              onChange={(e) => setFormValues({ ...formValues, targetDisplayName: e.target.value })}
              value={formValues.targetDisplayName}
            />
            <TextField
              fullWidth
              label="Data Type"
              onChange={(e) => setFormValues({ ...formValues, dataType: e.target.value })}
              select
              value={formValues.dataType}
            >
              {dataTypes.map((dt) => (
                <MenuItem key={dt} value={dt}>
                  {dt}
                </MenuItem>
              ))}
            </TextField>
            <TextField
              fullWidth
              label="Entity Type"
              onChange={(e) => setFormValues({ ...formValues, entityType: e.target.value })}
              select
              value={formValues.entityType}
            >
              {entityTypes.map((et) => (
                <MenuItem key={et} value={et}>
                  {et === 'employee-import' ? 'Employee Import' : 'Resource Allocation'}
                </MenuItem>
              ))}
            </TextField>
            <TextField
              fullWidth
              label="Display Order"
              onChange={(e) => {
                const val = Number(e.target.value);
                if (val >= 0) setFormValues({ ...formValues, displayOrder: val });
              }}
              type="number"
              value={formValues.displayOrder}
              slotProps={{ htmlInput: { min: 0 } }}
            />
            <Stack alignItems="center" direction="row" spacing={1}>
              <Typography variant="body2">Required:</Typography>
              <input
                checked={formValues.isRequired}
                onChange={(e) => setFormValues({ ...formValues, isRequired: e.target.checked })}
                type="checkbox"
              />
            </Stack>
            <Stack alignItems="center" direction="row" spacing={1}>
              <Typography variant="body2">Active:</Typography>
              <input
                checked={formValues.isActive}
                onChange={(e) => setFormValues({ ...formValues, isActive: e.target.checked })}
                type="checkbox"
              />
            </Stack>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleSave} variant="contained">Save</Button>
        </DialogActions>
      </Dialog>

      <Dialog onClose={() => setDeleteTarget(null)} open={Boolean(deleteTarget)}>
        <DialogTitle>Delete Column Mapping?</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete mapping for <strong>{deleteTarget?.sourceColumn}</strong>?
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteTarget(null)}>Cancel</Button>
          <Button color="error" onClick={handleDelete} variant="contained">Delete</Button>
        </DialogActions>
      </Dialog>
    </PageContainer>
  );
}
