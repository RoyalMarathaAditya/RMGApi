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
import type { CreateRoleDto, RoleDto } from '../types/userManagement';
import { roleService } from '../services/roleService';

const defaultForm: CreateRoleDto = { name: '', description: '', isActive: true };

export default function Roles() {
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<RoleDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<RoleDto | null>(null);
  const [form, setForm] = useState<CreateRoleDto>(defaultForm);
  const [formError, setFormError] = useState('');
  const [saving, setSaving] = useState(false);

  const loadData = useCallback(async () => {
    setLoading(true);
    try {
      const data = await roleService.getRoles();
      setRoles(data);
    } catch { toastService.error('Failed to load roles'); }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { loadData(); }, [loadData]);

  const handleOpenAdd = () => { setEditTarget(null); setForm(defaultForm); setFormError(''); setDialogOpen(true); };

  const handleOpenEdit = (r: RoleDto) => { setEditTarget(r); setForm({ name: r.name, description: r.description ?? '', isActive: r.isActive }); setFormError(''); setDialogOpen(true); };

  const handleSave = async () => {
    if (!form.name.trim()) { setFormError('Name is required'); return; }
    setFormError(''); setSaving(true);
    try {
      if (editTarget) {
        await roleService.updateRole(editTarget.id, form);
        toastService.success('Role updated successfully');
      } else {
        await roleService.createRole(form);
        toastService.success('Role created successfully');
      }
      setDialogOpen(false);
      loadData();
    } catch (err: any) { setFormError(err?.response?.data?.message || 'Failed to save role'); }
    finally { setSaving(false); }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await roleService.deleteRole(deleteTarget.id);
      toastService.success('Role deleted successfully');
      setDeleteTarget(null);
      loadData();
    } catch (err: any) { toastService.error(err?.response?.data?.message || 'Failed to delete role'); }
  };

  const handleToggleActive = async (role: RoleDto) => {
    try {
      if (role.isActive) {
        await roleService.deactivateRole(role.id);
        toastService.success('Role deactivated successfully');
      } else {
        await roleService.activateRole(role.id);
        toastService.success('Role activated successfully');
      }
      loadData();
    } catch { toastService.error('Failed to update role status'); }
  };

  return (
    <PageContainer title="Roles">
      <Stack spacing={3}>
        <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
          <Typography color="text.secondary" variant="body2">{roles.length} role(s)</Typography>
          <Button onClick={handleOpenAdd} startIcon={<AddIcon />} variant="contained">Add Role</Button>
        </Stack>

        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
          <TableContainer>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell sx={{ fontWeight: 700 }}>Role Name</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Description</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>No. of Users</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Status</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Created Date</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {loading ? (
                  <TableRow><TableCell colSpan={6} align="center" sx={{ py: 4 }}><Typography color="text.secondary">Loading...</Typography></TableCell></TableRow>
                ) : roles.map((r) => (
                  <TableRow key={r.id} hover>
                    <TableCell><Typography fontWeight={600}>{r.name}</Typography></TableCell>
                    <TableCell>{r.description ?? '-'}</TableCell>
                    <TableCell>{r.userCount}</TableCell>
                    <TableCell><Chip color={r.isActive ? 'success' : 'default'} label={r.isActive ? 'Active' : 'Inactive'} size="small" variant="outlined" /></TableCell>
                    <TableCell>{new Date(r.createdOn).toLocaleDateString()}</TableCell>
                    <TableCell>
                      <IconButton color="primary" onClick={() => handleOpenEdit(r)} size="small"><EditIcon fontSize="small" /></IconButton>
                      <IconButton onClick={() => handleToggleActive(r)} size="small">
                        <Chip color={r.isActive ? 'warning' : 'success'} label={r.isActive ? 'Deactivate' : 'Activate'} size="small" variant="outlined" />
                      </IconButton>
                      <IconButton color="error" onClick={() => setDeleteTarget(r)} size="small"><DeleteIcon fontSize="small" /></IconButton>
                    </TableCell>
                  </TableRow>
                ))}
                {!loading && roles.length === 0 && (
                  <TableRow><TableCell colSpan={6} align="center" sx={{ py: 4 }}><Typography color="text.secondary">No roles found</Typography></TableCell></TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
      </Stack>

      <Dialog fullWidth maxWidth="sm" onClose={() => setDialogOpen(false)} open={dialogOpen}>
        <DialogTitle>{editTarget ? 'Edit Role' : 'Add Role'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            {formError && <Alert severity="error">{formError}</Alert>}
            <TextField label="Role Name" required value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
            <TextField label="Description" multiline rows={3} value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
            <Stack direction="row" spacing={1} alignItems="center">
              <Typography variant="body2">Active:</Typography>
              <input type="checkbox" checked={form.isActive} onChange={(e) => setForm({ ...form, isActive: e.target.checked })} />
            </Stack>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleSave} disabled={saving} variant="contained">{saving ? 'Saving...' : 'Save'}</Button>
        </DialogActions>
      </Dialog>

      <Dialog onClose={() => setDeleteTarget(null)} open={Boolean(deleteTarget)}>
        <DialogTitle>Delete Role?</DialogTitle>
        <DialogContent>
          <Typography>Are you sure you want to delete <strong>{deleteTarget?.name}</strong>?<br />This action cannot be undone if no users are assigned to this role.</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteTarget(null)}>Cancel</Button>
          <Button color="error" onClick={handleDelete} variant="contained">Delete</Button>
        </DialogActions>
      </Dialog>
    </PageContainer>
  );
}