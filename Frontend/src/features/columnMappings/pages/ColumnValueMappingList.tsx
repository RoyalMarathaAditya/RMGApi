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
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from '@mui/material';
import { useCallback, useEffect, useState } from 'react';
import PageContainer from '../../../components/common/PageContainer';
import { toastService } from '../../../services/toastService';
import { columnValueMappingService } from '../services/columnMappingService';
import type { ColumnValueMapping, ColumnValueMappingFormValues } from '../types/columnMapping';

const defaultFormValues: ColumnValueMappingFormValues = {
  targetProperty: '',
  sourceValue: '',
  targetValue: '',
  isActive: true,
};

export default function ColumnValueMappingList() {
  const [mappings, setMappings] = useState<ColumnValueMapping[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchText, setSearchText] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<ColumnValueMapping | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<ColumnValueMapping | null>(null);
  const [formValues, setFormValues] = useState<ColumnValueMappingFormValues>(defaultFormValues);
  const [formError, setFormError] = useState('');

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      const data = await columnValueMappingService.getAll();
      setMappings(data);
    } catch {
      setError('Failed to load value mappings');
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
      !q ||
      m.targetProperty.toLowerCase().includes(q) ||
      m.sourceValue.toLowerCase().includes(q) ||
      m.targetValue.toLowerCase().includes(q)
    );
  });

  const handleOpenAdd = () => {
    setEditing(null);
    setFormValues(defaultFormValues);
    setFormError('');
    setDialogOpen(true);
  };

  const handleOpenEdit = (m: ColumnValueMapping) => {
    setEditing(m);
    setFormValues({
      targetProperty: m.targetProperty,
      sourceValue: m.sourceValue,
      targetValue: m.targetValue,
      isActive: m.isActive,
    });
    setFormError('');
    setDialogOpen(true);
  };

  const handleSave = async () => {
    if (!formValues.targetProperty.trim() || !formValues.sourceValue.trim() || !formValues.targetValue.trim()) {
      setFormError('All fields are required');
      return;
    }
    setFormError('');
    try {
      if (editing) {
        await columnValueMappingService.update(editing.id, formValues);
        toastService.success('Value mapping updated successfully');
      } else {
        await columnValueMappingService.create(formValues);
        toastService.success('Value mapping created successfully');
      }
      setDialogOpen(false);
      await fetchData();
    } catch (err: any) {
      setFormError(err?.message || 'Failed to save value mapping');
    }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await columnValueMappingService.delete(deleteTarget.id);
      setDeleteTarget(null);
      toastService.success('Value mapping deleted successfully');
      await fetchData();
    } catch {
      toastService.error('Failed to delete value mapping');
    }
  };

  return (
    <PageContainer title="Column Value Mappings">
      <Stack spacing={3}>
        <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
          <Box>
            <Typography color="text.secondary" variant="body2">
              {filtered.length} of {mappings.length} value mappings
            </Typography>
          </Box>
          <Button onClick={handleOpenAdd} startIcon={<AddIcon />} variant="contained">
            Add Value Mapping
          </Button>
        </Stack>

        <TextField
          fullWidth
          label="Search value mappings"
          onChange={(e) => setSearchText(e.target.value)}
          value={searchText}
        />

        {error ? <Alert severity="error">{error}</Alert> : null}

        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell sx={{ fontWeight: 700 }}>Target Property</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Source Value</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Target Value</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Status</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {filtered.map((m) => (
                  <TableRow key={m.id} hover>
                    <TableCell>
                      <Chip color="primary" label={m.targetProperty} size="small" variant="outlined" />
                    </TableCell>
                    <TableCell>{m.sourceValue}</TableCell>
                    <TableCell>{m.targetValue}</TableCell>
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
                    <TableCell colSpan={5} align="center">
                      <Typography color="text.secondary" py={3}>
                        No value mappings found
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
        <DialogTitle>{editing ? 'Edit Value Mapping' : 'Add Value Mapping'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            {formError && <Alert severity="error">{formError}</Alert>}
            <TextField
              fullWidth
              label="Target Property"
              onChange={(e) => setFormValues({ ...formValues, targetProperty: e.target.value })}
              required
              value={formValues.targetProperty}
            />
            <TextField
              fullWidth
              label="Source Value"
              onChange={(e) => setFormValues({ ...formValues, sourceValue: e.target.value })}
              required
              value={formValues.sourceValue}
            />
            <TextField
              fullWidth
              label="Target Value"
              onChange={(e) => setFormValues({ ...formValues, targetValue: e.target.value })}
              required
              value={formValues.targetValue}
            />
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
        <DialogTitle>Delete Value Mapping?</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete value mapping <strong>{deleteTarget?.sourceValue} &rarr; {deleteTarget?.targetValue}</strong>?
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
