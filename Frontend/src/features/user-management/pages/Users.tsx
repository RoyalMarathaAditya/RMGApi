import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import FileDownloadOutlinedIcon from '@mui/icons-material/FileDownloadOutlined';
import LockIcon from '@mui/icons-material/Lock';
import LockOpenIcon from '@mui/icons-material/LockOpen';
import RemoveRedEyeOutlinedIcon from '@mui/icons-material/RemoveRedEyeOutlined';
import ReplayIcon from '@mui/icons-material/Replay';
import SearchIcon from '@mui/icons-material/Search';
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
import React, { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import PageContainer from '../../../components/common/PageContainer';
import TableLoader from '../../../components/common/TableLoader';
import { useDebounce } from '../../../hooks/useDebounce';
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

const sortableHeaders = ['Employee', 'Role', 'Created Date', 'Last Login'] as const;
const sortMap: Record<string, string> = { 'Employee': 'name', 'Role': 'role', 'Created Date': 'createdat', 'Last Login': 'lastlogindate' };

interface ConfirmState {
  open: boolean;
  title: string;
  message: string;
  confirmLabel: string;
  confirmColor?: 'error' | 'warning' | 'primary' | 'success';
  onConfirm: () => void;
}

const ActionsCell = React.memo(function ActionsCell({
  user,
  onView,
  onEdit,
  onLock,
  onUnlock,
  onActivate,
  onDeactivate,
  onDelete,
  onResetPwd,
  onResetDefault,
}: {
  user: UserListDto;
  onView: (u: UserListDto) => void;
  onEdit: (u: UserListDto) => void;
  onLock: (id: number) => void;
  onUnlock: (id: number) => void;
  onActivate: (id: number) => void;
  onDeactivate: (id: number) => void;
  onDelete: (u: UserListDto) => void;
  onResetPwd: (u: UserListDto) => void;
  onResetDefault: (id: number) => void;
}) {
  return (
    <Stack direction="row" spacing={0.5}>
      <Tooltip title="View"><IconButton size="small" onClick={() => onView(user)}><RemoveRedEyeOutlinedIcon fontSize="small" /></IconButton></Tooltip>
      <Tooltip title="Edit"><IconButton size="small" onClick={() => onEdit(user)}><EditIcon fontSize="small" /></IconButton></Tooltip>
      {user.isActive && !user.isLocked && (
        <Tooltip title="Lock">
          <IconButton size="small" onClick={() => onLock(user.id)}><LockIcon fontSize="small" /></IconButton>
        </Tooltip>
      )}
      {user.isLocked && (
        <Tooltip title="Unlock">
          <IconButton size="small" onClick={() => onUnlock(user.id)}><LockOpenIcon fontSize="small" /></IconButton>
        </Tooltip>
      )}
      {user.isActive ? (
        <Tooltip title="Deactivate">
          <IconButton size="small" color="warning" onClick={() => onDeactivate(user.id)}><LockOpenIcon fontSize="small" /></IconButton>
        </Tooltip>
      ) : !user.isLocked && (
        <Tooltip title="Activate">
          <IconButton size="small" color="success" onClick={() => onActivate(user.id)}><LockOpenIcon fontSize="small" /></IconButton>
        </Tooltip>
      )}
      <Tooltip title="Reset Password"><IconButton size="small" onClick={() => onResetPwd(user)}><LockOpenIcon fontSize="small" /></IconButton></Tooltip>
      <Tooltip title="Reset to Default">
        <IconButton size="small" color="warning" onClick={() => onResetDefault(user.id)}><ReplayIcon fontSize="small" /></IconButton>
      </Tooltip>
      <Tooltip title="Delete"><IconButton size="small" color="error" onClick={() => onDelete(user)}><DeleteIcon fontSize="small" /></IconButton></Tooltip>
    </Stack>
  );
});

const UserRow = React.memo(function UserRow({
  user,
  onView,
  onEdit,
  onLock,
  onUnlock,
  onActivate,
  onDeactivate,
  onDelete,
  onResetPwd,
  onResetDefault,
}: {
  user: UserListDto;
  onView: (u: UserListDto) => void;
  onEdit: (u: UserListDto) => void;
  onLock: (id: number) => void;
  onUnlock: (id: number) => void;
  onActivate: (id: number) => void;
  onDeactivate: (id: number) => void;
  onDelete: (u: UserListDto) => void;
  onResetPwd: (u: UserListDto) => void;
  onResetDefault: (id: number) => void;
}) {
  return (
    <TableRow hover>
      <TableCell sx={{ maxWidth: 260, minWidth: 180 }}>
        <Typography fontWeight={600} variant="body2" sx={{ whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>
          {user.name}
        </Typography>
      </TableCell>
      <TableCell>{user.email}</TableCell>
      <TableCell><Chip label={user.roleName} size="small" variant="outlined" /></TableCell>
      <TableCell>
        <Chip
          label={user.isDefaultPassword && user.isFirstLogin ? 'Default Password' : 'Password Changed'}
          size="small"
          color={user.isDefaultPassword && user.isFirstLogin ? 'warning' : 'success'}
          variant="filled"
        />
      </TableCell>
      <TableCell><UserStatusChip isActive={user.isActive} isLocked={user.isLocked} /></TableCell>
      <TableCell>{user.lastLoginDate ? new Date(user.lastLoginDate).toLocaleDateString() : '-'}</TableCell>
      <TableCell>{new Date(user.createdAt).toLocaleDateString()}</TableCell>
      <TableCell>
        <ActionsCell
          user={user}
          onView={onView}
          onEdit={onEdit}
          onLock={onLock}
          onUnlock={onUnlock}
          onActivate={onActivate}
          onDeactivate={onDeactivate}
          onDelete={onDelete}
          onResetPwd={onResetPwd}
          onResetDefault={onResetDefault}
        />
      </TableCell>
    </TableRow>
  );
});

export default function Users() {
  const [data, setData] = useState<PagedResponse<UserListDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState('');
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [rawSearchTerm, setRawSearchTerm] = useState('');
  const debouncedSearch = useDebounce(rawSearchTerm, 400);
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
  const [refreshCounter, setRefreshCounter] = useState(0);
  const abortRef = useRef<AbortController | null>(null);
  const initialLoadDone = useRef(false);

  const refresh = useCallback(() => setRefreshCounter(c => c + 1), []);

  const loadDataAsync = useCallback(async (signal: AbortSignal) => {
    setLoading(true);
    setLoadError('');
    try {
      const params: PaginationParams = {
        pageNumber: page + 1,
        pageSize: rowsPerPage,
        searchTerm: debouncedSearch || undefined,
        sortBy,
        sortDescending: sortDesc,
        roleIdFilter: roleFilter || undefined,
        statusFilter: statusFilter || undefined,
      };
      const result = await userService.getUsers(params);
      if (!signal.aborted) {
        setData(result);
      }
    } catch (err: any) {
      if (!signal.aborted) {
        const message = err?.response?.data?.message || err?.message || 'Failed to load users';
        setLoadError(message);
        toastService.error(message);
      }
    } finally {
      if (!signal.aborted) {
        setLoading(false);
      }
    }
  }, [page, rowsPerPage, debouncedSearch, sortBy, sortDesc, roleFilter, statusFilter, refreshCounter]);

  useEffect(() => {
    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;
    loadDataAsync(controller.signal);
    return () => { controller.abort(); };
  }, [loadDataAsync]);

  useEffect(() => {
    if (initialLoadDone.current) return;
    initialLoadDone.current = true;

    const loadInitial = async () => {
      setRolesLoading(true);
      setRolesError('');
      try {
        const rolesRes = await roleService.getRoles();
        setRoles(rolesRes);
      } catch {
        setRolesError('Unable to load roles');
        setRoles([]);
      } finally {
        setRolesLoading(false);
      }
    };
    loadInitial();
  }, []);

  const showConfirm = useCallback((title: string, message: string, confirmLabel: string, onConfirm: () => void, confirmColor?: 'error' | 'warning' | 'primary' | 'success') => {
    setConfirm({ open: true, title, message, confirmLabel, confirmColor, onConfirm });
  }, []);

  const handleOpenAdd = useCallback(async () => {
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
  }, [roles]);

  const handleEmployeeChange = useCallback((employee: AvailableEmployee | null) => {
    setCreateForm({
      ...createForm,
      employeeId: employee?.employeeId ?? null,
      name: employee?.employeeName ?? '',
      userName: employee?.email ?? '',
      email: employee?.email ?? '',
      phone: employee?.mobileNumber ?? '',
    });
    setEmployeeDetail(null);
  }, [createForm]);

  const handleEdit = useCallback((user: UserListDto) => {
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
  }, []);

  const handleSaveAdd = useCallback(async () => {
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
      refresh();
    } catch (err: any) {
      setFormError(err?.response?.data?.message || 'Failed to create user');
    } finally { setSaving(false); }
  }, [createForm, refresh]);

  const handleSaveEdit = useCallback(async () => {
    if (!editTarget) return;
    if (!editForm.roleId) { setFormError('Please select a role'); return; }

    const dto: UpdateUserDto = { roleId: editForm.roleId, isActive: editForm.isActive };
    if (editForm.phone !== editTarget.phone) {
      dto.phone = editForm.phone || '';
    }

    setFormError(''); setSaving(true);
    try {
      await userService.updateUser(editTarget.id, dto);
      toastService.success('User updated successfully');
      setEditTarget(null);
      refresh();
    } catch (err: any) {
      setFormError(err?.response?.data?.message || 'Failed to update user');
    } finally { setSaving(false); }
  }, [editTarget, editForm, refresh]);

  const handleDelete = useCallback(async () => {
    if (!deleteTarget) return;
    try {
      await userService.deleteUser(deleteTarget.id);
      toastService.success('User deleted successfully');
      setDeleteTarget(null);
      refresh();
    } catch { toastService.error('Failed to delete user'); }
  }, [deleteTarget, refresh]);

  const execWithRefresh = useCallback(async (promise: Promise<unknown>, successMsg: string) => {
    try {
      await promise;
      toastService.success(successMsg);
    } catch {
      toastService.error('Operation failed');
    } finally {
      refresh();
    }
  }, [refresh]);

  const handleLockUser = useCallback((id: number) => {
    showConfirm('Lock User', 'Are you sure you want to lock this user?', 'Lock', () => execWithRefresh(userService.lockUser(id), 'User locked successfully'), 'warning');
  }, [showConfirm, execWithRefresh]);
  const handleUnlockUser = useCallback((id: number) => {
    showConfirm('Unlock User', 'Are you sure you want to unlock this user?', 'Unlock', () => execWithRefresh(userService.unlockUser(id), 'User unlocked successfully'), 'warning');
  }, [showConfirm, execWithRefresh]);
  const handleActivateUser = useCallback((id: number) => {
    showConfirm('Activate User', 'Are you sure you want to activate this user?', 'Activate', () => execWithRefresh(userService.activateUser(id), 'User activated successfully'), 'success');
  }, [showConfirm, execWithRefresh]);
  const handleDeactivateUser = useCallback((id: number) => {
    showConfirm('Deactivate User', 'Are you sure you want to deactivate this user?', 'Deactivate', () => execWithRefresh(userService.deactivateUser(id), 'User deactivated successfully'), 'error');
  }, [showConfirm, execWithRefresh]);

  const handleResetPwdDefault = useCallback((id: number) => {
    showConfirm(
      'Reset Password to Default',
      'Reset this user\'s password to the default password (NV@12345#)? The user will be forced to change it on next login.',
      'Reset to Default',
      () => execWithRefresh(userService.resetPasswordToDefault(id), 'Password reset to default'),
      'warning'
    );
  }, [showConfirm, execWithRefresh]);

  const handleResetPwd = useCallback(async () => {
    if (!resetPwdForm.newPassword || resetPwdForm.newPassword !== resetPwdForm.confirmPassword) {
      setFormError('Passwords do not match');
      return;
    }
    setFormError(''); setSaving(true);
    try {
      await userService.resetPassword(resetPwdForm);
      toastService.success('Password reset successfully');
      setResetPwdTarget(null);
      refresh();
    } catch (err: any) {
      setFormError(err?.response?.data?.message || 'Failed to reset password');
    } finally { setSaving(false); }
  }, [resetPwdForm, refresh]);

  const handleExport = useCallback(() => {
    toastService.info('Export feature coming soon');
  }, []);

  const handleViewUser = useCallback((u: UserListDto) => setViewUser(u), []);
  const handleEditUser = useCallback((u: UserListDto) => handleEdit(u), [handleEdit]);
  const handleDeleteUser = useCallback((u: UserListDto) => setDeleteTarget(u), []);
  const handleResetPwdUser = useCallback((u: UserListDto) => {
    setResetPwdTarget(u);
    setResetPwdForm({ userId: u.id, newPassword: '', confirmPassword: '' });
    setFormError('');
  }, []);

  const handleSortClick = useCallback((header: string) => {
    const field = sortMap[header];
    if (!field) return;
    setSortBy(prev => {
      if (prev === field) setSortDesc(d => !d);
      else setSortDesc(false);
      return field;
    });
  }, []);

  const handleSearchChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    setRawSearchTerm(e.target.value);
    setPage(0);
  }, []);

  const handleRoleFilterChange = useCallback((e: any) => {
    setRoleFilter(e.target.value);
    setPage(0);
  }, []);

  const handleStatusFilterChange = useCallback((e: any) => {
    setStatusFilter(e.target.value);
    setPage(0);
  }, []);

  const sortableHeadersSet = useMemo(() => new Set(sortableHeaders), []);

  return (
    <PageContainer title="Users">
      <Stack spacing={3}>
        <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
          <Typography color="text.secondary" variant="body2">
            {data ? `${data.totalCount} user(s)` : loading ? '' : ''}
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
            value={rawSearchTerm}
            onChange={handleSearchChange}
            slotProps={{
              input: {
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon color="action" fontSize="small" />
                  </InputAdornment>
                ),
              },
            }}
          />
          <Select disabled={loading} size="small" value={roleFilter} onChange={handleRoleFilterChange} displayEmpty sx={{ minWidth: 140 }}>
            <MenuItem value="">All Roles</MenuItem>
            {rolesLoading ? (
              <MenuItem disabled>Loading...</MenuItem>
            ) : rolesError ? (
              <MenuItem disabled>Error loading roles</MenuItem>
            ) : (
              roles.map(r => <MenuItem key={r.id} value={r.id}>{r.name}</MenuItem>)
            )}
          </Select>
          <Select disabled={loading} size="small" value={statusFilter} onChange={handleStatusFilterChange} displayEmpty sx={{ minWidth: 140 }}>
            <MenuItem value="">All Status</MenuItem>
            <MenuItem value="active">Active</MenuItem>
            <MenuItem value="inactive">Inactive</MenuItem>
            <MenuItem value="locked">Locked</MenuItem>
          </Select>
        </Stack>

        {loadError && !loading && (
          <Alert
            action={<Button color="inherit" onClick={() => loadDataAsync(new AbortController().signal)} size="small" startIcon={<ReplayIcon />}>Retry</Button>}
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
                      sx={{
                        fontWeight: 700,
                        cursor: sortableHeadersSet.has(h as any) ? 'pointer' : 'default',
                        userSelect: 'none',
                      }}
                      onClick={() => handleSortClick(h)}
                    >
                      <Stack direction="row" alignItems="center" spacing={0.5}>
                        <span>{h}</span>
                        {sortBy === sortMap[h] && (
                          <Typography variant="caption" color="primary">
                            {sortDesc ? '↓' : '↑'}
                          </Typography>
                        )}
                      </Stack>
                    </TableCell>
                  ))}
                </TableRow>
              </TableHead>
              {loading ? (
                <TableLoader columns={8} rows={rowsPerPage > 8 ? 8 : rowsPerPage} />
              ) : (
                <TableBody>
                  {data?.items.map((u) => (
                    <UserRow
                      key={u.id}
                      user={u}
                      onView={handleViewUser}
                      onEdit={handleEditUser}
                      onLock={handleLockUser}
                      onUnlock={handleUnlockUser}
                      onActivate={handleActivateUser}
                      onDeactivate={handleDeactivateUser}
                      onDelete={handleDeleteUser}
                      onResetPwd={handleResetPwdUser}
                      onResetDefault={handleResetPwdDefault}
                    />
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

      <Dialog onClose={() => setConfirm({ ...confirm, open: false })} open={confirm.open}>
        <DialogTitle>{confirm.title}</DialogTitle>
        <DialogContent><Typography>{confirm.message}</Typography></DialogContent>
        <DialogActions>
          <Button onClick={() => setConfirm({ ...confirm, open: false })}>Cancel</Button>
          <Button color={confirm.confirmColor || 'primary'} onClick={() => { confirm.onConfirm(); setConfirm({ ...confirm, open: false }); }} variant="contained">
            {confirm.confirmLabel}
          </Button>
        </DialogActions>
      </Dialog>

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

const Row = React.memo(function Row({ label, value }: { label: string; value?: string | null }) {
  return (
    <Stack direction="row" justifyContent="space-between">
      <Typography color="text.secondary" variant="body2">{label}</Typography>
      <Typography fontWeight={500} variant="body2">{value ?? '-'}</Typography>
    </Stack>
  );
});
