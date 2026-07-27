import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import FileDownloadOutlinedIcon from '@mui/icons-material/FileDownloadOutlined';
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
  CircularProgress,
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
import UserStatusChip from '../components/UserStatusChip';
import type {
  AvailableEmployee,
  CreateUserDto,
  EmployeeDetail,
  PagedResponse,
  PaginationParams,
  ResetPasswordDto,
  RoleDto,
  UpdateUserDto,
  UserListDto,
} from '../types/userManagement';
import { userService } from '../services/userService';
import { roleService } from '../services/roleService';

const defaultCreateForm: CreateUserDto = {
  employeeId: null,
  userName: '',
  name: '',
  email: '',
  phone: '',
  roleId: '',
  isActive: true,
};

const defaultEditForm = {
  employeeId: null as number | null,
  name: '',
  userName: '',
  email: '',
  phone: '',
  roleId: '',
  isActive: true,
};

interface ConfirmState {
  open: boolean;
  title: string;
  message: string;
  confirmLabel: string;
  confirmColor?: 'error' | 'warning' | 'primary' | 'success';
  onConfirm: () => void;
}

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
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [rolesLoading, setRolesLoading] = useState(true);
  const [rolesError, setRolesError] = useState('');
  const [availableEmployees, setAvailableEmployees] = useState<AvailableEmployee[]>([]);
  const [employeeDetail, setEmployeeDetail] = useState<EmployeeDetail | null>(null);
  const [viewUser, setViewUser] = useState<UserListDto | null>(null);
  const [editTarget, setEditTarget] = useState<UserListDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<UserListDto | null>(null);
  const [addDialogOpen, setAddDialogOpen] = useState(false);
  const [resetPwdTarget, setResetPwdTarget] = useState<UserListDto | null>(null);
  const [createForm, setCreateForm] = useState<CreateUserDto>(defaultCreateForm);
  const [editForm, setEditForm] = useState(defaultEditForm);
  const [resetPwdForm, setResetPwdForm] = useState<ResetPasswordDto>({ userId: 0, newPassword: '', confirmPassword: '' });
  const [formError, setFormError] = useState('');
  const [saving, setSaving] = useState(false);
  const [confirm, setConfirm] = useState<ConfirmState>({ open: false, title: '', message: '', confirmLabel: '', onConfirm: () => {} });
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
        roleIdFilter: roleFilter || undefined,
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
      setRolesLoading(true);
      setRolesError('');
      try {
        const res = await roleService.getRoles();
        setRoles(res);
      } catch {
        setRolesError('Unable to load roles');
        setRoles([]);
      } finally {
        setRolesLoading(false);
      }
    };
    loadRoles();
  }, []);

  const showConfirm = (title: string, message: string, confirmLabel: string, onConfirm: () => void, confirmColor?: 'error' | 'warning' | 'primary' | 'success') => {
    setConfirm({ open: true, title, message, confirmLabel, confirmColor, onConfirm });
  };

  const handleOpenAdd = async () => {
    const employeeRole = roles.find(r => r.name === 'Employee');
    setCreateForm({ ...defaultCreateForm, roleId: employeeRole?.id ?? '' });
    setEmployeeDetail(null);
    setFormError('');
    setAddDialogOpen(true);
    try {
      const employees = await userService.getDropdownEmployees();
      setAvailableEmployees(employees);
    } catch {
      setAvailableEmployees([]);
      toastService.error('Failed to load employees');
    }
  };

  const handleEmployeeChange = async (employee: AvailableEmployee | null) => {
    setCreateForm({
      ...createForm,
      employeeId: employee?.employeeId ?? null,
      name: employee?.employeeName ?? '',
      userName: employee?.email ?? '',
      email: employee?.email ?? '',
      phone: employee?.mobileNumber ?? '',
    });
    setEmployeeDetail(null);
  };

  const handleEdit = async (user: UserListDto) => {
    setEditTarget(user);
    setEditForm({
      employeeId: user.employeeId ?? null,
      name: user.name,
      userName: user.userName ?? user.email,
      email: user.email,
      phone: user.phone ?? '',
      roleId: user.roleId,
      isActive: user.isActive,
    });
    setFormError('');
  };

  const handleSaveAdd = async () => {
    if (!createForm.employeeId) { setFormError('Please select an employee'); return; }
    if (!createForm.roleId) { setFormError('Please select a role'); return; }
    if (!createForm.email.trim()) { setFormError('Email is required'); return; }
    if (!createForm.name.trim()) { setFormError('Name is required'); return; }
    if (!createForm.userName.trim()) { setFormError('Employee Code is required'); return; }
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
    if (!editForm.roleId) { setFormError('Please select a role'); return; }
    if (!editForm.email.trim()) { setFormError('Email is required'); return; }

    const dto: UpdateUserDto = {
      roleId: editForm.roleId,
      isActive: editForm.isActive,
    };

    if (editForm.phone !== editTarget.phone) {
      dto.phone = editForm.phone || '';
    }

    setFormError(''); setSaving(true);
    try {
      await userService.updateUser(editTarget.id, dto);
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

  const handleResetPwdDefault = async (id: number) => {
    try {
      const result = await userService.resetPasswordToDefault(id);
      toastService.success(result.message || 'Password reset to default');
      loadData();
    } catch (err: any) {
      toastService.error(err?.response?.data?.message || 'Failed to reset password');
    }
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

  const handleExport = () => {
    toastService.info('Export feature coming soon');
  };

  return (
    <PageContainer title="Users">
      <Stack spacing={3}>
        <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
          <Typography color="text.secondary" variant="body2">
            {data ? `${data.totalCount} user(s)` : ''}
          </Typography>
          <Stack direction="row" spacing={1}>
            <Button disabled={loading} onClick={handleExport} startIcon={<FileDownloadOutlinedIcon />} variant="outlined">
              Export
            </Button>
            <Button disabled={loading} onClick={handleOpenAdd} startIcon={<AddIcon />} variant="contained">
              Add User
            </Button>
          </Stack>
        </Stack>

        <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
          <TextField
            disabled={loading}
            fullWidth
            placeholder="Search by name, email..."
            value={searchTerm}
            onChange={(e) => { setSearchTerm(e.target.value); setPage(0); }}
            slotProps={{ input: { startAdornment: <InputAdornment position="start"><Typography color="text.secondary">🔍</Typography></InputAdornment> } }}
          />
          <Select disabled={loading} size="small" value={roleFilter} onChange={(e) => { setRoleFilter(e.target.value); setPage(0); }} displayEmpty sx={{ minWidth: 140 }}>
            <MenuItem value="">All Roles</MenuItem>
            {roles.map(r => <MenuItem key={r.id} value={r.id}>{r.name}</MenuItem>)}
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
                  {['Employee', 'Email', 'Role', 'Password Status', 'Status', 'Last Login', 'Created Date', 'Actions'].map((h) => (
                    <TableCell
                      key={h}
                      sx={{ fontWeight: 700, cursor: ['Name', 'Role', 'Created Date', 'Last Login'].includes(h) ? 'pointer' : 'default' }}
                      onClick={() => {
                        const sortMap: Record<string, string> = { 'Name': 'name', 'Role': 'role', 'Created Date': 'createdat', 'Last Login': 'lastlogindate' };
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
                      <TableCell sx={{ maxWidth: 260, minWidth: 180 }}>
                        <Typography
                          fontWeight={600}
                          variant="body2"
                          sx={{ whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}
                        >
                          {u.name}
                        </Typography>
                      </TableCell>
                      <TableCell>{u.email}</TableCell>
                      <TableCell><Chip label={u.roleName} size="small" variant="outlined" /></TableCell>
                      <TableCell>
                        <Chip
                          label={u.isDefaultPassword && u.isFirstLogin ? 'Default Password' : 'Password Changed'}
                          size="small"
                          color={u.isDefaultPassword && u.isFirstLogin ? 'warning' : 'success'}
                          variant="filled"
                        />
                      </TableCell>
                      <TableCell><UserStatusChip isActive={u.isActive} isLocked={u.isLocked} /></TableCell>
                      <TableCell>{u.lastLoginDate ? new Date(u.lastLoginDate).toLocaleDateString() : '-'}</TableCell>
                      <TableCell>{new Date(u.createdAt).toLocaleDateString()}</TableCell>
                      <TableCell>
                        <Stack direction="row" spacing={0.5}>
                          <Tooltip title="View"><IconButton size="small" onClick={() => setViewUser(u)}><RemoveRedEyeOutlinedIcon fontSize="small" /></IconButton></Tooltip>
                          <Tooltip title="Edit"><IconButton size="small" onClick={() => handleEdit(u)}><EditIcon fontSize="small" /></IconButton></Tooltip>
                          {u.isActive && !u.isLocked && (
                            <Tooltip title="Lock">
                              <IconButton size="small" onClick={() => showConfirm('Lock User', `Are you sure you want to lock ${u.name}?`, 'Lock', () => handleLock(u.id), 'warning')}>
                                <LockIcon fontSize="small" />
                              </IconButton>
                            </Tooltip>
                          )}
                          {u.isLocked && (
                            <Tooltip title="Unlock">
                              <IconButton size="small" onClick={() => showConfirm('Unlock User', `Are you sure you want to unlock ${u.name}?`, 'Unlock', () => handleUnlock(u.id), 'warning')}>
                                <LockOpenIcon fontSize="small" />
                              </IconButton>
                            </Tooltip>
                          )}
                          {u.isActive ? (
                            <Tooltip title="Deactivate">
                              <IconButton size="small" color="warning" onClick={() => showConfirm('Deactivate User', `Are you sure you want to deactivate ${u.name}?`, 'Deactivate', () => handleDeactivate(u.id), 'error')}>
                                <LockOpenIcon fontSize="small" />
                              </IconButton>
                            </Tooltip>
                          ) : !u.isLocked && (
                            <Tooltip title="Activate">
                              <IconButton size="small" color="success" onClick={() => showConfirm('Activate User', `Are you sure you want to activate ${u.name}?`, 'Activate', () => handleActivate(u.id), 'success')}>
                                <LockOpenIcon fontSize="small" />
                              </IconButton>
                            </Tooltip>
                          )}
                          <Tooltip title="Reset Password"><IconButton size="small" onClick={() => { setResetPwdTarget(u); setResetPwdForm({ userId: u.id, newPassword: '', confirmPassword: '' }); setFormError(''); }}><LockOpenIcon fontSize="small" /></IconButton></Tooltip>
                          <Tooltip title="Reset to Default">
                            <IconButton size="small" color="warning" onClick={() => showConfirm(
                              'Reset Password to Default',
                              `Reset ${u.name}'s password to the default password (NV@12345#)? The user will be forced to change it on next login.`,
                              'Reset to Default',
                              () => handleResetPwdDefault(u.id),
                              'warning'
                            )}>
                              <ReplayIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
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

      {/* Unified Confirmation Dialog */}
      <Dialog onClose={() => setConfirm({ ...confirm, open: false })} open={confirm.open}>
        <DialogTitle>{confirm.title}</DialogTitle>
        <DialogContent>
          <Typography>{confirm.message}</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setConfirm({ ...confirm, open: false })}>Cancel</Button>
          <Button color={confirm.confirmColor || 'primary'} onClick={() => { confirm.onConfirm(); setConfirm({ ...confirm, open: false }); }} variant="contained">
            {confirm.confirmLabel}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Add User Dialog */}
      <Dialog fullWidth maxWidth="md" onClose={() => setAddDialogOpen(false)} open={addDialogOpen}>
        <DialogTitle>Add User</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            {formError && <Alert severity="error">{formError}</Alert>}
            <Autocomplete
              options={availableEmployees}
              getOptionLabel={(o) => `${o.employeeName} (${o.employeeCode})`}
              onChange={(_, v) => handleEmployeeChange(v)}
              renderInput={(params) => <TextField {...params} label="Search Employee *" placeholder="Type to search..." />}
            />
            <TextField label="Employee Code" disabled value={availableEmployees.find(e => e.employeeId === createForm.employeeId)?.employeeCode ?? '-'} />
            <TextField label="Employee Name" disabled value={createForm.name} />
            <TextField label="Email" required type="email" disabled value={createForm.email} />
            <TextField label="Contact No" value={createForm.phone} onChange={(e) => setCreateForm({ ...createForm, phone: e.target.value })} />
            <TextField label="Department" disabled value={employeeDetail?.departmentType ?? '-'} />
            <TextField label="Designation" disabled value={employeeDetail?.designation ?? '-'} />
            {rolesLoading ? (
              <Stack direction="row" alignItems="center" spacing={1}>
                <CircularProgress size={20} />
                <Typography variant="body2" color="text.secondary">Loading roles...</Typography>
              </Stack>
            ) : rolesError ? (
              <Alert severity="warning">{rolesError}</Alert>
            ) : (
              <Select value={createForm.roleId} onChange={(e) => setCreateForm({ ...createForm, roleId: e.target.value })} displayEmpty>
                <MenuItem value="" disabled>Select Role</MenuItem>
                {roles.map(r => <MenuItem key={r.id} value={r.id}>{r.name}</MenuItem>)}
              </Select>
            )}
            <Alert severity="info" sx={{ borderRadius: 2 }}>
              User will be created with the default password <strong>NV@12345#</strong>. They will be required to change it on first login.
            </Alert>
            <Stack direction="row" spacing={1} alignItems="center">
              <Typography variant="body2">Is Active:</Typography>
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
      <Dialog fullWidth maxWidth="md" onClose={() => setEditTarget(null)} open={Boolean(editTarget)}>
        <DialogTitle>Edit User - {editTarget?.name}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            {formError && <Alert severity="error">{formError}</Alert>}
            <TextField label="Employee Name" disabled value={editForm.name} />
            <TextField label="Email" required type="email" disabled value={editForm.email} />
            <TextField label="Contact No" value={editForm.phone} onChange={(e) => setEditForm({ ...editForm, phone: e.target.value })} />
            {rolesLoading ? (
              <Stack direction="row" alignItems="center" spacing={1}>
                <CircularProgress size={20} />
                <Typography variant="body2" color="text.secondary">Loading roles...</Typography>
              </Stack>
            ) : rolesError ? (
              <Alert severity="warning">{rolesError}</Alert>
            ) : (
              <Select value={editForm.roleId} onChange={(e) => setEditForm({ ...editForm, roleId: e.target.value })} displayEmpty>
                <MenuItem value="" disabled>Select Role</MenuItem>
                {roles.map(r => <MenuItem key={r.id} value={r.id}>{r.name}</MenuItem>)}
              </Select>
            )}
            {editTarget?.isDefaultPassword && editTarget?.isFirstLogin ? (
              <Alert severity="info" sx={{ borderRadius: 2 }}>
                Default Password: <strong>NV@12345#</strong> (User has not logged in yet)
              </Alert>
            ) : (
              <Alert severity="success" sx={{ borderRadius: 2 }}>
                Password already changed by user.
              </Alert>
            )}
            <Stack direction="row" spacing={1} alignItems="center">
              <Typography variant="body2">Is Active:</Typography>
              <input type="checkbox" checked={editForm.isActive} onChange={(e) => setEditForm({ ...editForm, isActive: e.target.checked })} />
            </Stack>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditTarget(null)}>Cancel</Button>
          <Button onClick={handleSaveEdit} disabled={saving} variant="contained">{saving ? 'Saving...' : 'Update'}</Button>
        </DialogActions>
      </Dialog>

      {/* Delete Confirmation */}
      <Dialog onClose={() => setDeleteTarget(null)} open={Boolean(deleteTarget)}>
        <DialogTitle>Delete User?</DialogTitle>
        <DialogContent>
          <Typography>Are you sure you want to delete <strong>{deleteTarget?.name}</strong>? This action performs a soft delete and can be reversed.</Typography>
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
              <Row label="Employee Code" value={viewUser?.employeeCode ?? viewUser?.userName} />
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
              <Row label="Role" value={viewUser?.roleName} />
              <Row label="Status" value={viewUser?.isLocked ? 'Locked' : viewUser?.isActive ? 'Active' : 'Inactive'} />
              <Row label="Password Status" value={viewUser?.isDefaultPassword && viewUser?.isFirstLogin ? 'Default Password' : 'Password Changed'} />
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
