import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import RefreshIcon from '@mui/icons-material/Refresh';
import SearchIcon from '@mui/icons-material/Search';
import {
  Alert,
  Box,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  IconButton,
  InputAdornment,
  InputLabel,
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
  TableSortLabel,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material';
import { useEffect, useMemo, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
import PageContainer from '../../../components/common/PageContainer';
import {
  createProject,
  deleteProject,
  fetchClients,
  fetchProjects,
  fetchRevenueTypes,
  updateProject,
} from '../../../redux/slices/projectManagementSlice';
import type { Project, CreateProjectRequest } from '../types/project';
import { toastService } from '../../../services/toastService';

type SortField = 'projectName' | 'projectCode' | 'clientName' | 'csmRevenueTypeName' | 'deliveryHead' | 'projectManager' | 'csm' | 'isActive';
type SortDir = 'asc' | 'desc';

const defaultFormValues: CreateProjectRequest = {
  projectName: '',
  projectCode: '',
  clientId: 0,
  projectManager: '',
  deliveryHead: '',
  csm: '',
  csmRevenueTypeId: null,
  isActive: true,
  description: '',
};

export default function ProjectManagementList() {
  const dispatch = useAppDispatch();
  const { projects, clients, revenueTypes, loading, error } = useAppSelector((state) => state.projectManagement);

  const [searchText, setSearchText] = useState('');

  const [sortField, setSortField] = useState<SortField>('projectName');
  const [sortDir, setSortDir] = useState<SortDir>('asc');

  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<Project | null>(null);
  const [formValues, setFormValues] = useState<CreateProjectRequest>(defaultFormValues);
  const [formError, setFormError] = useState('');
  const [deleteTarget, setDeleteTarget] = useState<Project | null>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    dispatch(fetchProjects());
    if (clients.length === 0) {
      dispatch(fetchClients());
    }
    if (revenueTypes.length === 0) {
      dispatch(fetchRevenueTypes());
    }
  }, [dispatch, clients.length, revenueTypes.length]);

  const handleSort = (field: SortField) => {
    if (sortField === field) {
      setSortDir((prev) => (prev === 'asc' ? 'desc' : 'asc'));
    } else {
      setSortField(field);
      setSortDir('asc');
    }
  };

  const filtered = useMemo(() => {
    const q = searchText.trim().toLowerCase();
    let list = projects;
    if (q) {
      list = list.filter((p) =>
        [p.projectName, p.projectCode, p.clientName, p.deliveryHead, p.projectManager, p.csm]
          .some((field) => (field ?? '').toLowerCase().includes(q)),
      );
    }
    list = [...list].sort((a, b) => {
      const aVal = String(a[sortField] ?? '');
      const bVal = String(b[sortField] ?? '');
      const cmp = aVal.localeCompare(bVal);
      return sortDir === 'asc' ? cmp : -cmp;
    });
    return list;
  }, [projects, searchText, sortField, sortDir]);

  const paginated = useMemo(
    () => filtered.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage),
    [filtered, page, rowsPerPage],
  );

  const handleChangePage = (_: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const handleRefresh = () => {
    dispatch(fetchProjects());
  };

  const handleOpenAdd = () => {
    setEditTarget(null);
    setFormValues(defaultFormValues);
    setFormError('');
    setDialogOpen(true);
  };

  const handleOpenEdit = (project: Project) => {
    setEditTarget(project);
    setFormValues({
      projectName: project.projectName,
      projectCode: project.projectCode ?? '',
      clientId: project.clientId,
      projectManager: project.projectManager ?? '',
      deliveryHead: project.deliveryHead ?? '',
      csm: project.csm ?? '',
      csmRevenueTypeId: project.csmRevenueTypeId ?? null,
      isActive: project.isActive,
      description: project.description ?? '',
    });
    setFormError('');
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setEditTarget(null);
    setFormError('');
  };

  const validateForm = (): boolean => {
    if (!formValues.projectName.trim()) {
      setFormError('Project Name is required');
      return false;
    }
    if (formValues.projectName.trim().length > 200) {
      setFormError('Project Name must not exceed 200 characters');
      return false;
    }
    if (!formValues.clientId) {
      setFormError('Client is required');
      return false;
    }
    return true;
  };

  const handleSave = async () => {
    if (!validateForm()) return;
    setSaving(true);
    setFormError('');
    try {
      const values: CreateProjectRequest = {
        projectName: formValues.projectName.trim(),
        projectCode: formValues.projectCode?.trim() || null,
        clientId: formValues.clientId,
        projectManager: formValues.projectManager?.trim() || null,
        deliveryHead: formValues.deliveryHead?.trim() || null,
        csm: formValues.csm?.trim() || null,
        csmRevenueTypeId: formValues.csmRevenueTypeId || null,
        isActive: formValues.isActive,
        description: formValues.description?.trim() || null,
      };

      if (editTarget) {
        await dispatch(updateProject({ id: editTarget.id, values })).unwrap();
        toastService.success('Project updated successfully');
      } else {
        await dispatch(createProject(values)).unwrap();
        toastService.success('Project created successfully');
      }
      handleCloseDialog();
    } catch (err: any) {
      setFormError(err?.message || 'Failed to save project');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await dispatch(deleteProject(deleteTarget.id)).unwrap();
      setDeleteTarget(null);
      toastService.success('Project deleted successfully');
    } catch {
      toastService.error('Failed to delete project');
    }
  };

  const SortableHeader = ({ field, label }: { field: SortField; label: string }) => (
    <TableCell sx={{ fontWeight: 700, whiteSpace: 'nowrap' }}>
      <TableSortLabel active={sortField === field} direction={sortField === field ? sortDir : 'asc'} onClick={() => handleSort(field)}>
        {label}
      </TableSortLabel>
    </TableCell>
  );

  return (
    <PageContainer title="Project Management">
      <Stack spacing={3}>
        <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
          <Box>
            <Typography color="text.secondary" variant="body2">
              {filtered.length} of {projects.length} projects
            </Typography>
          </Box>
          <Stack direction="row" spacing={1.5}>
            <Tooltip title="Refresh">
              <Button onClick={handleRefresh} startIcon={<RefreshIcon />} variant="outlined">
                Refresh
              </Button>
            </Tooltip>
            <Button onClick={handleOpenAdd} startIcon={<AddIcon />} variant="contained">
              Add Project
            </Button>
          </Stack>
        </Stack>

        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
          <TextField
            fullWidth
            label="Search projects"
            onChange={(e) => {
              setSearchText(e.target.value);
              setPage(0);
            }}
            slotProps={{
              input: {
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon />
                  </InputAdornment>
                ),
              },
            }}
            value={searchText}
          />
        </Stack>

        {error ? <Alert severity="error">{error}</Alert> : null}

        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <SortableHeader field="projectName" label="Project Name" />
                  <SortableHeader field="projectCode" label="Project Code" />
                  <SortableHeader field="clientName" label="Client Name" />
                  <SortableHeader field="csmRevenueTypeName" label="Revenue Type" />
                  <SortableHeader field="deliveryHead" label="Delivery Head" />
                  <SortableHeader field="projectManager" label="Project Manager" />
                  <SortableHeader field="csm" label="CSM" />
                  <SortableHeader field="isActive" label="Status" />
                  <TableCell sx={{ fontWeight: 700 }}>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {loading ? (
                  <TableRow>
                    <TableCell align="center" colSpan={9}>
                      <Typography color="text.secondary" py={3}>
                        Loading...
                      </Typography>
                    </TableCell>
                  </TableRow>
                ) : paginated.length === 0 ? (
                  <TableRow>
                    <TableCell align="center" colSpan={9}>
                      <Typography color="text.secondary" py={3}>
                        No projects found
                      </Typography>
                    </TableCell>
                  </TableRow>
                ) : (
                  paginated.map((project) => (
                    <TableRow key={project.id} hover>
                      <TableCell sx={{ fontWeight: 600 }}>{project.projectName}</TableCell>
                      <TableCell>{project.projectCode || '-'}</TableCell>
                      <TableCell>{project.clientName}</TableCell>
                      <TableCell>{project.csmRevenueTypeName || '-'}</TableCell>
                      <TableCell>{project.deliveryHead || '-'}</TableCell>
                      <TableCell>{project.projectManager || '-'}</TableCell>
                      <TableCell>{project.csm || '-'}</TableCell>
                      <TableCell>
                        <Chip
                          color={project.isActive ? 'success' : 'default'}
                          label={project.isActive ? 'Active' : 'Inactive'}
                          size="small"
                          variant="outlined"
                        />
                      </TableCell>
                      <TableCell>
                        <Stack direction="row" spacing={0.5}>
                          <Tooltip title="Edit">
                            <IconButton color="primary" onClick={() => handleOpenEdit(project)} size="small">
                              <EditIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                          <Tooltip title="Delete">
                            <IconButton color="error" onClick={() => setDeleteTarget(project)} size="small">
                              <DeleteIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                        </Stack>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </TableContainer>
          <TablePagination
            component="div"
            count={filtered.length}
            onPageChange={handleChangePage}
            onRowsPerPageChange={handleChangeRowsPerPage}
            page={page}
            rowsPerPage={rowsPerPage}
            rowsPerPageOptions={[5, 10, 25, 50]}
          />
        </Paper>
      </Stack>

      {/* Add / Edit Dialog */}
      <Dialog fullWidth maxWidth="sm" onClose={handleCloseDialog} open={dialogOpen}>
        <DialogTitle>{editTarget ? 'Edit Project' : 'Add Project'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2.5} sx={{ mt: 1 }}>
            {formError && <Alert severity="error">{formError}</Alert>}
            <TextField
              fullWidth
              label="Project Name"
              onChange={(e) => setFormValues({ ...formValues, projectName: e.target.value })}
              required
              value={formValues.projectName}
            />
            <TextField
              fullWidth
              label="Project Code"
              onChange={(e) => setFormValues({ ...formValues, projectCode: e.target.value })}
              value={formValues.projectCode}
            />
            <TextField
              fullWidth
              label="Client"
              onChange={(e) => setFormValues({ ...formValues, clientId: Number(e.target.value) })}
              required
              select
              value={formValues.clientId}
            >
              <MenuItem value={0} disabled>
                Select Client
              </MenuItem>
              {clients.map((c) => (
                <MenuItem key={c.id} value={c.id}>
                  {c.name}
                </MenuItem>
              ))}
            </TextField>
            <TextField
              fullWidth
              label="Revenue Type"
              onChange={(e) => setFormValues({ ...formValues, csmRevenueTypeId: e.target.value || null })}
              select
              value={formValues.csmRevenueTypeId ?? ''}
            >
              <MenuItem value="">None</MenuItem>
              {revenueTypes.map((rt) => (
                <MenuItem key={rt.id} value={rt.id}>
                  {rt.name}
                </MenuItem>
              ))}
            </TextField>
            <TextField
              fullWidth
              label="Delivery Head"
              onChange={(e) => setFormValues({ ...formValues, deliveryHead: e.target.value })}
              value={formValues.deliveryHead}
            />
            <TextField
              fullWidth
              label="Project Manager"
              onChange={(e) => setFormValues({ ...formValues, projectManager: e.target.value })}
              value={formValues.projectManager}
            />
            <TextField
              fullWidth
              label="CSM"
              onChange={(e) => setFormValues({ ...formValues, csm: e.target.value })}
              value={formValues.csm}
            />
            <TextField
              fullWidth
              label="Description"
              multiline
              minRows={3}
              onChange={(e) => setFormValues({ ...formValues, description: e.target.value })}
              value={formValues.description}
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
          <Button disabled={saving} onClick={handleCloseDialog}>
            Cancel
          </Button>
          <Button disabled={saving} onClick={handleSave} variant="contained">
            {saving ? 'Saving...' : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Delete Confirmation */}
      <Dialog onClose={() => setDeleteTarget(null)} open={Boolean(deleteTarget)}>
        <DialogTitle>Delete Project?</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete <strong>{deleteTarget?.projectName}</strong>?
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteTarget(null)}>Cancel</Button>
          <Button color="error" onClick={handleDelete} variant="contained">
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    </PageContainer>
  );
}
