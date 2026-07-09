import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import LockIcon from '@mui/icons-material/Lock';
import LockOpenIcon from '@mui/icons-material/LockOpen';
import RemoveRedEyeOutlinedIcon from '@mui/icons-material/RemoveRedEyeOutlined';
import ReplayIcon from '@mui/icons-material/Replay';
import {
  Alert,
  Autocomplete,
  Box,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Drawer,
  IconButton,
  InputAdornment,
  MenuItem,
  Paper,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material';
import { useCallback, useEffect, useRef, useState } from 'react';
import PageContainer from '../../../components/common/PageContainer';
import TableLoader from '../../../components/common/TableLoader';
import { toastService } from '../../../services/toastService';
import type {
  AvailableEmployee,
  CreateUserDto,
  PagedResponse,
  PaginationParams,
  ResetPasswordDto,
  UpdateUserDto,
  UserListDto,
} from '../types/userManagement';
import { userService } from '../services/userService';
import { roleService } from '../services/roleService';

const statusColors: Record<string, 'success' | 'error' | 'warning' | 'default'> = {
  Active: 'success',
  Inactive: 'error',
  Locked: 'warning',
};

const defaultCreateForm: CreateUserDto = {
  employeeId: null,
  userName: '',
  name: '',
  email: '',
  phone: '',
  password: '',
  confirmPassword: '',
  role: 'Employee',
  isActive: true,
};

export default function Users() {
  const [data, setData] = useState<PagedResponse<UserListDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState('');
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [searchTerm, setSearchTerm] = useState('');
  const [roleFilter, setRoleFilter] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [sortBy, setSortBy] = useState('name');
  const [sortDesc, setSortDesc] = useState(false);
  const [roles, setRoles] = useState<string[]>([]);
  const [availableEmployees, setAvailableEmployees] = useState<AvailableEmployee[]>([]);
  const [viewUser, setViewUser] = useState<UserListDto | null>(null);
  const [editTarget, setEditTarget] = useState<UserListDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<UserListDto | null>(null);
  const [addDialogOpen, setAddDialogOpen] = useState(false);
  const [resetPwdTarget, setResetPwdTarget] = useState<UserListDto | null>(null);
  const [createForm, setCreateForm] = useState<CreateUserDto>(defaultCreateForm);
  const [editForm, setEditForm] = useState<UpdateUserDto>({});
  const [resetPwdForm, setResetPwdForm] = useState<ResetPasswordDto>({ userId: 0, newPassword: '', confirmPassword: '' });
  const [formError, setFormError] = useState('');
  const [saving, setSaving] = useState(false);
  const abortRef = useRef<AbortController | null>(null);

  const loadData = useCallback(async () => {
    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;

    setLoading(true);
    setLoadError('');
    try {
      const params: PaginationParams = {
        pageNumber: page + 1,
        pageSize: rowsPerPage,
        searchTerm: searchTerm || undefined,
        sortBy,
        sortDescending: sortDesc,
        roleFilter: roleFilter || undefined,
        statusFilter: statusFilter || undefined,
      };
      const result = await userService.getUsers(params);
      if (!controller.signal.aborted) {
        setData(result);
      }
    } catch (err: any) {
      if (!controller.signal.aborted) {
        const message = err?.response?.data?.message || err?.message || 'Failed to load users';
        setLoadError(message);
        toastService.error(message);
      }
    } finally {
      if (!controller.signal.aborted) {
        setLoading(false);
      }
    }
  }, [page, rowsPerPage, searchTerm, sortBy, sortDesc, roleFilter, statusFilter]);

  useEffect(() => {
    loadData();
    return () => {
      abortRef.current?.abort();
    };
  }, [loadData]);

  useEffect(() => {
    const loadRoles = async () => {
      try {
        const res = await roleService.getRoles();
        setRoles(res.map(r => r.name));
      } catch { /* ignore */ }
    };
    loadRoles();
  }, []);

  const handleOpenAdd = async () => {
    setCreateForm(defaultCreateForm);
    setFormError('');
    try {
      const employees = await userService.getAvailableEmployees();
      setAvailableEmployees(employees);
    } catch { /* ignore */ }
    setAddDialogOpen(true);
  };

  const handleEdit = (user: UserListDto) => {
    setEditTarget(user);
    setEditForm({ phone: user.phone ?? '', email: user.email, role: user.role, isActive: user.isActive });
    setFormError('');
  };

  const handleSaveAdd = async () => {
    if (!createForm.userName.trim()) { setFormError('Username is required'); return; }
    if (!createForm.name.trim()) { setFormError('Name is required'); return; }
    if (!createForm.email.trim()) { setFormError('Email is required'); return; }
    if (!createForm.password) { setFormError('Password is required'); return; }
    if (createForm.password !== createForm.confirmPassword) { setFormError('Passwords do not match'); return; }
    setFormError(''); setSaving(true);
    try {
      await userService.createUser(createForm);
      toastService.success('User created successfully');
      setAddDialogOpen(false);
      loadData();
    } catch (err: any) {
      setFormError(err?.response?.data?.message || 'Failed to create user');
    } finally { setSaving(false); }
  };

  const handleSaveEdit = async () => {
    if (!editTarget) return;
    setFormError(''); setSaving(true);
    try {
      await userService.updateUser(editTarget.id, editForm);
      toastService.success('User updated successfully');
      setEditTarget(null);
      loadData();
    } catch (err: any) {
      setFormError(err?.response?.data?.message || 'Failed to update user');
    } finally { setSaving(false); }
  };

  const handleLock = async (id: number) => {
    try {
      await userService.lockUser(id);
      toastService.success('User locked successfully');
      loadData();
    } catch { toastService.error('Failed to lock user'); }
  };

  const handleUnlock = async (id: number) => {
    try {
      await userService.unlockUser(id);
      toastService.success('User unlocked successfully');
      loadData();
    } catch { toastService.error('Failed to unlock user'); }
  };

  const handleActivate = async (id: number) => {
    try {
      await userService.activateUser(id);
      toastService.success('User activated successfully');
      loadData();
    } catch { toastService.error('Failed to activate user'); }
  };

  const handleDeactivate = async (id: number) => {
    try {
      await userService.deactivateUser(id);
      toastService.success('User deactivated successfully');
      loadData();
    } catch { toastService.error('Failed to deactivate user'); }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await userService.deleteUser(deleteTarget.id);
      toastService.success('User deleted successfully');
      setDeleteTarget(null);
      loadData();
    } catch { toastService.error('Failed to delete user'); }
  };

  const handleResetPwd = async () => {
    if (!resetPwdForm.newPassword || resetPwdForm.newPassword !== resetPwdForm.confirmPassword) {
      setFormError('Passwords do not match');
      return;
    }
    setFormError(''); setSaving(true);
    try {
      await userService.resetPassword(resetPwdForm);
      toastService.success('Password reset successfully');
      setResetPwdTarget(null);
    } catch (err: any) {
      setFormError(err?.response?.data?.message || 'Failed to reset password');
    } finally { setSaving(false); }
  };

  return (
    <PageContainer title="Users">
      <Stack spacing={3}>
        <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
          <Typography color="text.secondary" variant="body2">
            {data ? `${data.totalCount} user(s)` : ''}
          </Typography>
          <Button disabled={loading} onClick={handleOpenAdd} startIcon={<AddIcon />} variant="contained">
            Add User
          </Button>
        </Stack>

        <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
          <TextField
            disabled={loading}
            fullWidth
            placeholder="Search by name, username, email..."
            value={searchTerm}
            onChange={(e) => { setSearchTerm(e.target.value); setPage(0); }}
            slotProps={{ input: { startAdornment: <InputAdornment position="start"><Typography color="text.secondary">🔍</Typography></InputAdornment> } }}
          />
          <Select disabled={loading} size="small" value={roleFilter} onChange={(e) => { setRoleFilter(e.target.value); setPage(0); }} displayEmpty sx={{ minWidth: 140 }}>
            <MenuItem value="">All Roles</MenuItem>
            {roles.map(r => <MenuItem key={r} value={r}>{r}</MenuItem>)}
          </Select>
          <Select disabled={loading} size="small" value={statusFilter} onChange={(e) => { setStatusFilter(e.target.value); setPage(0); }} displayEmpty sx={{ minWidth: 140 }}>
            <MenuItem value="">All Status</MenuItem>
            <MenuItem value="active">Active</MenuItem>
            <MenuItem value="inactive">Inactive</MenuItem>
            <MenuItem value="locked">Locked</MenuItem>
          </Select>
        </Stack>

        {loadError && !loading && (
          <Alert
            action={<Button color="inherit" onClick={loadData} size="small" startIcon={<ReplayIcon />}>Retry</Button>}
            severity="error"
            sx={{ borderRadius: 2 }}
          >
            {loadError}
          </Alert>
        )}

        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
          <TableContainer>
            <Table size="small">
              <TableHead>
                <TableRow>
                  {['Employee', 'Username', 'Email', 'Role', 'Status', 'Last Login', 'Created Date', 'Actions'].map((h) => (
                    <TableCell
                      key={h}
                      sx={{ fontWeight: 700, cursor: ['Name', 'Username', 'Role', 'Created Date', 'Last Login'].includes(h) ? 'pointer' : 'default' }}
                      onClick={() => {
                        const sortMap: Record<string, string> = { 'Name': 'name', 'Username': 'username', 'Role': 'role', 'Created Date': 'createdat', 'Last Login': 'lastlogindate' };
                        const field = sortMap[h];
                        if (field) {
                          if (sortBy === field) setSortDesc(!sortDesc);
                          else { setSortBy(field); setSortDesc(false); }
                        }
                      }}
                    >
                      {h}
                    </TableCell>
                  ))}
                </TableRow>
              </TableHead>
              {loading ? (
                <TableLoader columns={8} rows={8} />
              ) : (
                <TableBody>
                  {data?.items.map((u) => (
                    <TableRow key={u.id} hover>
                      <TableCell>
                        <Typography fontWeight={600} variant="body2">{u.name}</Typography>
                        <Typography color="text.secondary" variant="caption">{u.employeeCode ? `${u.employeeCode}` : ''}</Typography>
                      </TableCell>
                      <TableCell>{u.userName ?? '-'}</TableCell>
                      <TableCell>{u.email}</TableCell>
                      <TableCell><Chip label={u.role} size="small" variant="outlined" /></TableCell>
                      <TableCell>
                        <Chip
                          label={u.isLocked ? 'Locked' : u.isActive ? 'Active' : 'Inactive'}
                          color={u.isLocked ? 'warning' : u.isActive ? 'success' : 'error'}
                          size="small"
                          variant="outlined"
                        />
                      </TableCell>
                      <TableCell>{u.lastLoginDate ? new Date(u.lastLoginDate).toLocaleDateString() : '-'}</TableCell>
                      <TableCell>{new Date(u.createdAt).toLocaleDateString()}</TableCell>
                      <TableCell>
                        <Stack direction="row" spacing={0.5}>
                          <Tooltip title="View"><IconButton size="small" onClick={() => setViewUser(u)}><RemoveRedEyeOutlinedIcon fontSize="small" /></IconButton></Tooltip>
                          <Tooltip title="Edit"><IconButton size="small" onClick={() => handleEdit(u)}><EditIcon fontSize="small" /></IconButton></Tooltip>
                          <Tooltip title={u.isLocked ? 'Unlock' : 'Lock'}>
                            <IconButton size="small" onClick={() => u.isLocked ? handleUnlock(u.id) : handleLock(u.id)}>
                              {u.isLocked ? <LockOpenIcon fontSize="small" /> : <LockIcon fontSize="small" />}
                            </IconButton>
                          </Tooltip>
                          <Tooltip title="Reset Password"><IconButton size="small" onClick={() => { setResetPwdTarget(u); setResetPwdForm({ userId: u.id, newPassword: '', confirmPassword: '' }); setFormError(''); }}><LockOpenIcon fontSize="small" /></IconButton></Tooltip>
                          <Tooltip title="Delete"><IconButton size="small" color="error" onClick={() => setDeleteTarget(u)}><DeleteIcon fontSize="small" /></IconButton></Tooltip>
                        </Stack>
                      </TableCell>
                    </TableRow>
                  ))}
                  {data?.items.length === 0 && (
                    <TableRow><TableCell colSpan={8} align="center" sx={{ py: 4 }}><Typography color="text.secondary">No users found.</Typography></TableCell></TableRow>
                  )}
                </TableBody>
              )}
            </Table>
          </TableContainer>
          <TablePagination
            component="div"
            count={data?.totalCount ?? 0}
            page={page}
            onPageChange={(_, p) => setPage(p)}
            rowsPerPage={rowsPerPage}
            onRowsPerPageChange={(e) => { setRowsPerPage(Number(e.target.value)); setPage(0); }}
            rowsPerPageOptions={[10, 25, 50]}
            disabled={loading}
          />
        </Paper>
      </Stack>

      {/* Add User Dialog */}
      <Dialog fullWidth maxWidth="md" onClose={() => setAddDialogOpen(false)} open={addDialogOpen}>
        <DialogTitle>Add User</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            {formError && <Alert severity="error">{formError}</Alert>}
            <Autocomplete
              options={availableEmployees}
              getOptionLabel={(o) => `${o.fullName} (${o.employeeCode})`}
              onChange={(_, v) => setCreateForm({ ...createForm, employeeId: v?.id ?? null, name: v?.fullName ?? '' })}
              renderInput={(params) => <TextField {...params} label="Employee (optional)" />}
            />
            <TextField label="Username" required value={createForm.userName} onChange={(e) => setCreateForm({ ...createForm, userName: e.target.value })} />
            <TextField label="Name" required value={createForm.name} onChange={(e) => setCreateForm({ ...createForm, name: e.target.value })} />
            <TextField label="Email" required type="email" value={createForm.email} onChange={(e) => setCreateForm({ ...createForm, email: e.target.value })} />
            <TextField label="Phone" value={createForm.phone} onChange={(e) => setCreateForm({ ...createForm, phone: e.target.value })} />
            <Select value={createForm.role} onChange={(e) => setCreateForm({ ...createForm, role: e.target.value })}>
              {roles.map(r => <MenuItem key={r} value={r}>{r}</MenuItem>)}
            </Select>
            <TextField label="Password" required type="password" value={createForm.password} onChange={(e) => setCreateForm({ ...createForm, password: e.target.value })} />
            <TextField label="Confirm Password" required type="password" value={createForm.confirmPassword} onChange={(e) => setCreateForm({ ...createForm, confirmPassword: e.target.value })} />
            <Stack direction="row" spacing={1} alignItems="center">
              <Typography variant="body2">Active:</Typography>
              <input type="checkbox" checked={createForm.isActive} onChange={(e) => setCreateForm({ ...createForm, isActive: e.target.checked })} />
            </Stack>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setAddDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleSaveAdd} disabled={saving} variant="contained">{saving ? 'Saving...' : 'Save'}</Button>
        </DialogActions>
      </Dialog>

      {/* Edit User Dialog */}
      <Dialog fullWidth maxWidth="sm" onClose={() => setEditTarget(null)} open={Boolean(editTarget)}>
        <DialogTitle>Edit User - {editTarget?.name}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            {formError && <Alert severity="error">{formError}</Alert>}
            <TextField label="Email" type="email" value={editForm.email ?? ''} onChange={(e) => setEditForm({ ...editForm, email: e.target.value })} />
            <TextField label="Phone" value={editForm.phone ?? ''} onChange={(e) => setEditForm({ ...editForm, phone: e.target.value })} />
            <Select size="small" value={editForm.role ?? ''} onChange={(e) => setEditForm({ ...editForm, role: e.target.value })}>
              {roles.map(r => <MenuItem key={r} value={r}>{r}</MenuItem>)}
            </Select>
            <Stack direction="row" spacing={1} alignItems="center">
              <Typography variant="body2">Active:</Typography>
              <input type="checkbox" checked={editForm.isActive ?? false} onChange={(e) => setEditForm({ ...editForm, isActive: e.target.checked })} />
            </Stack>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditTarget(null)}>Cancel</Button>
          <Button onClick={handleSaveEdit} disabled={saving} variant="contained">{saving ? 'Saving...' : 'Save'}</Button>
        </DialogActions>
      </Dialog>

      {/* Delete Confirmation */}
      <Dialog onClose={() => setDeleteTarget(null)} open={Boolean(deleteTarget)}>
        <DialogTitle>Delete User?</DialogTitle>
        <DialogContent>
          <Typography>Are you sure you want to delete <strong>{deleteTarget?.name}</strong>?</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteTarget(null)}>Cancel</Button>
          <Button color="error" onClick={handleDelete} variant="contained">Delete</Button>
        </DialogActions>
      </Dialog>

      {/* Reset Password Dialog */}
      <Dialog fullWidth maxWidth="sm" onClose={() => setResetPwdTarget(null)} open={Boolean(resetPwdTarget)}>
        <DialogTitle>Reset Password - {resetPwdTarget?.name}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            {formError && <Alert severity="error">{formError}</Alert>}
            <TextField label="New Password" required type="password" value={resetPwdForm.newPassword} onChange={(e) => setResetPwdForm({ ...resetPwdForm, newPassword: e.target.value })} />
            <TextField label="Confirm Password" required type="password" value={resetPwdForm.confirmPassword} onChange={(e) => setResetPwdForm({ ...resetPwdForm, confirmPassword: e.target.value })} />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setResetPwdTarget(null)}>Cancel</Button>
          <Button onClick={handleResetPwd} disabled={saving} variant="contained">{saving ? 'Resetting...' : 'Reset Password'}</Button>
        </DialogActions>
      </Dialog>

      {/* View User Drawer */}
      <Drawer anchor="right" onClose={() => setViewUser(null)} open={Boolean(viewUser)} PaperProps={{ sx: { width: { xs: 300, sm: 400, md: 500 } } }}>
        <Stack spacing={2} sx={{ p: 3 }}>
          <Typography fontWeight={700} variant="h6">{viewUser?.name}</Typography>
          <Box>
            <Typography color="text.secondary" sx={{ textTransform: 'uppercase', letterSpacing: 1, fontSize: 11, mb: 1 }}>Basic Information</Typography>
            <Stack spacing={1}>
              <Row label="Employee Name" value={viewUser?.employeeName} />
              <Row label="Employee Code" value={viewUser?.employeeCode} />
              <Row label="Username" value={viewUser?.userName} />
              <Row label="Email" value={viewUser?.email} />
              <Row label="Phone" value={viewUser?.phone} />
            </Stack>
          </Box>
          <Box>
            <Typography color="text.secondary" sx={{ textTransform: 'uppercase', letterSpacing: 1, fontSize: 11, mb: 1 }}>Employment Details</Typography>
            <Stack spacing={1}>
              <Row label="Designation" value={viewUser?.designation} />
              <Row label="Practice" value={viewUser?.practice} />
              <Row label="Department" value={viewUser?.department} />
            </Stack>
          </Box>
          <Box>
            <Typography color="text.secondary" sx={{ textTransform: 'uppercase', letterSpacing: 1, fontSize: 11, mb: 1 }}>Account Details</Typography>
            <Stack spacing={1}>
              <Row label="Role" value={viewUser?.role} />
              <Row label="Status" value={viewUser?.isLocked ? 'Locked' : viewUser?.isActive ? 'Active' : 'Inactive'} />
              <Row label="Last Login" value={viewUser?.lastLoginDate ? new Date(viewUser.lastLoginDate).toLocaleString() : '-'} />
              <Row label="Failed Login Count" value={String(viewUser?.failedLoginCount ?? 0)} />
            </Stack>
          </Box>
          <Box>
            <Typography color="text.secondary" sx={{ textTransform: 'uppercase', letterSpacing: 1, fontSize: 11, mb: 1 }}>Audit</Typography>
            <Stack spacing={1}>
              <Row label="Created By" value={viewUser?.createdBy} />
              <Row label="Created Date" value={viewUser?.createdAt ? new Date(viewUser.createdAt).toLocaleString() : '-'} />
              <Row label="Modified By" value={viewUser?.modifiedBy} />
              <Row label="Modified Date" value={viewUser?.modifiedOn ? new Date(viewUser.modifiedOn).toLocaleString() : '-'} />
            </Stack>
          </Box>
        </Stack>
      </Drawer>
    </PageContainer>
  );
}

function Row({ label, value }: { label: string; value?: string | null }) {
  return (
    <Stack direction="row" justifyContent="space-between">
      <Typography color="text.secondary" variant="body2">{label}</Typography>
      <Typography fontWeight={500} variant="body2">{value ?? '-'}</Typography>
    </Stack>
  );
}