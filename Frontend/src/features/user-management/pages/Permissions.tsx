import {
  Alert,
  Box,
  Button,
  Checkbox,
  Chip,
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
import { useCallback, useEffect, useMemo, useState } from 'react';
import PageContainer from '../../../components/common/PageContainer';
import { toastService } from '../../../services/toastService';
import type { PermissionDto, RoleDto } from '../types/userManagement';
import { permissionService } from '../services/permissionService';
import { roleService } from '../services/roleService';

export default function Permissions() {
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [permissions, setPermissions] = useState<PermissionDto[]>([]);
  const [selectedRole, setSelectedRole] = useState('');
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    const load = async () => {
      try {
        const [r, p] = await Promise.all([
          roleService.getRoles(),
          permissionService.getAllPermissions(),
        ]);
        setRoles(r);
        setPermissions(p);
      } catch { toastService.error('Failed to load data'); }
    };
    load();
  }, []);

  const loadRolePermissions = useCallback(async (roleName: string) => {
    try {
      const perms = await permissionService.getPermissionsByRole(roleName);
      setPermissions(perms);
    } catch { toastService.error('Failed to load role permissions'); }
  }, []);

  useEffect(() => {
    if (selectedRole) {
      loadRolePermissions(selectedRole);
    }
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
          <Select size="small" value={selectedRole} onChange={(e) => setSelectedRole(e.target.value)} sx={{ minWidth: 200 }}>
            <MenuItem value="">-- Select Role --</MenuItem>
            {roles.filter(r => r.isActive).map((r) => (
              <MenuItem key={r.id} value={r.name}>{r.name}</MenuItem>
            ))}
          </Select>
        </Stack>

        {selectedRole ? (
          <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
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
              <Button disabled={saving} onClick={handleSave} variant="contained">
                {saving ? 'Saving...' : 'Save Permissions'}
              </Button>
            </Stack>
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