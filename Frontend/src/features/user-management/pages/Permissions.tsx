import ReplayIcon from '@mui/icons-material/Replay';
import {
  Alert,
  Box,
  Button,
  Checkbox,
  CircularProgress,
  FormControlLabel,
  FormGroup,
  Paper,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material';
import MenuItem from '@mui/material/MenuItem';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import PageContainer from '../../../components/common/PageContainer';
import { toastService } from '../../../services/toastService';
import type { PermissionDto, RoleDto } from '../types/userManagement';
import { permissionService } from '../services/permissionService';
import { roleService } from '../services/roleService';

export default function Permissions() {
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [permissions, setPermissions] = useState<PermissionDto[]>([]);
  const [selectedRole, setSelectedRole] = useState('');
  const [loading, setLoading] = useState(true);
  const [permissionsLoading, setPermissionsLoading] = useState(false);
  const [loadError, setLoadError] = useState('');
  const [saving, setSaving] = useState(false);
  const abortRef = useRef<AbortController | null>(null);
  const permAbortRef = useRef<AbortController | null>(null);

  const loadInitialData = useCallback(async () => {
    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;

    setLoading(true);
    setLoadError('');
    try {
      const [r, p] = await Promise.all([
        roleService.getRoles(),
        permissionService.getAllPermissions(),
      ]);
      if (!controller.signal.aborted) {
        setRoles(r);
        setPermissions(p);
      }
    } catch (err: any) {
      if (!controller.signal.aborted) {
        const message = err?.response?.data?.message || err?.message || 'Failed to load data';
        setLoadError(message);
        toastService.error(message);
      }
    } finally {
      if (!controller.signal.aborted) {
        setLoading(false);
      }
    }
  }, []);

  useEffect(() => {
    loadInitialData();
    return () => {
      abortRef.current?.abort();
    };
  }, [loadInitialData]);

  const loadRolePermissions = useCallback(async (roleName: string) => {
    permAbortRef.current?.abort();
    const controller = new AbortController();
    permAbortRef.current = controller;

    setPermissionsLoading(true);
    try {
      const perms = await permissionService.getPermissionsByRole(roleName);
      if (!controller.signal.aborted) {
        setPermissions(perms);
      }
    } catch (err: any) {
      if (!controller.signal.aborted) {
        toastService.error(err?.response?.data?.message || err?.message || 'Failed to load role permissions');
      }
    } finally {
      if (!controller.signal.aborted) {
        setPermissionsLoading(false);
      }
    }
  }, []);

  useEffect(() => {
    if (selectedRole) {
      loadRolePermissions(selectedRole);
    }
    return () => {
      permAbortRef.current?.abort();
    };
  }, [selectedRole, loadRolePermissions]);

  const groupedPermissions = useMemo(() => {
    const groups: Record<string, PermissionDto[]> = {};
    for (const p of permissions) {
      const cat = p.category ?? 'General';
      if (!groups[cat]) groups[cat] = [];
      groups[cat].push(p);
    }
    return groups;
  }, [permissions]);

  const handleToggle = (id: number) => {
    setPermissions((prev) =>
      prev.map((p) => (p.id === id ? { ...p, hasPermission: !p.hasPermission } : p))
    );
  };

  const handleSelectAll = (category: string, checked: boolean) => {
    setPermissions((prev) =>
      prev.map((p) => (p.category === category || (!p.category && category === 'General') ? { ...p, hasPermission: checked } : p))
    );
  };

  const handleSave = async () => {
    if (!selectedRole) { toastService.warning('Please select a role'); return; }
    setSaving(true);
    try {
      await permissionService.saveRolePermissions({
        roleName: selectedRole,
        permissionIds: permissions.filter((p) => p.hasPermission).map((p) => p.id),
      });
      toastService.success('Role permissions updated successfully');
    } catch { toastService.error('Failed to save permissions'); }
    finally { setSaving(false); }
  };

  return (
    <PageContainer title="Permissions">
      <Stack spacing={3}>
        <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} alignItems="center">
          <Typography fontWeight={600}>Select Role:</Typography>
          <Select disabled={loading} size="small" value={selectedRole} onChange={(e) => setSelectedRole(e.target.value)} sx={{ minWidth: 200 }}>
            <MenuItem value="">-- Select Role --</MenuItem>
            {roles.filter(r => r.isActive).map((r) => (
              <MenuItem key={r.id} value={r.name}>{r.name}</MenuItem>
            ))}
          </Select>
        </Stack>

        {loadError && !loading && (
          <Alert
            action={<Button color="inherit" onClick={loadInitialData} size="small" startIcon={<ReplayIcon />}>Retry</Button>}
            severity="error"
            sx={{ borderRadius: 2 }}
          >
            {loadError}
          </Alert>
        )}

        {loading ? (
          <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 4, textAlign: 'center' }}>
            <CircularProgress />
          </Paper>
        ) : selectedRole ? (
          <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
            {permissionsLoading ? (
              <Stack alignItems="center" spacing={2} sx={{ py: 4 }}>
                <CircularProgress size={32} />
                <Typography color="text.secondary" variant="body2">Loading permissions...</Typography>
              </Stack>
            ) : (
              <Stack spacing={3}>
                {Object.entries(groupedPermissions).map(([category, perms]) => {
                  const allChecked = perms.every((p) => p.hasPermission);
                  return (
                    <Box key={category}>
                      <Stack direction="row" alignItems="center" spacing={2} sx={{ mb: 1 }}>
                        <Typography fontWeight={700} variant="subtitle1">{category}</Typography>
                        <Button size="small" onClick={() => handleSelectAll(category, !allChecked)} variant="text">
                          {allChecked ? 'Deselect All' : 'Select All'}
                        </Button>
                      </Stack>
                      <FormGroup row>
                        {perms.map((p) => (
                          <FormControlLabel
                            key={p.id}
                            control={
                              <Checkbox
                                checked={p.hasPermission}
                                onChange={() => handleToggle(p.id)}
                                size="small"
                              />
                            }
                            label={
                              <Stack direction="row" spacing={1} alignItems="center">
                                <Typography variant="body2">{p.name}</Typography>
                              </Stack>
                            }
                            sx={{ minWidth: 200, mb: 0.5 }}
                          />
                        ))}
                      </FormGroup>
                    </Box>
                  );
                })}
                <Button disabled={saving || permissionsLoading} onClick={handleSave} variant="contained">
                  {saving ? 'Saving...' : 'Save Permissions'}
                </Button>
              </Stack>
            )}
          </Paper>
        ) : (
          <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 4, textAlign: 'center' }}>
            <Typography color="text.secondary">Select a role to manage its permissions</Typography>
          </Paper>
        )}
      </Stack>
    </PageContainer>
  );
}
