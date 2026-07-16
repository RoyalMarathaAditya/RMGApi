import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
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
  IconButton,
  InputAdornment,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TextField,
  Typography,
} from '@mui/material';
import { useEffect, useMemo, useState } from 'react';
// Redux: dispatches thunks for designation CRUD, reads designation list and loading state
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
import PageContainer from '../../../components/common/PageContainer';
import {
  createDesignation,
  deleteDesignation,
  fetchDesignations,
  updateDesignation,
} from '../../../redux/slices/designationSlice';
import type { Designation, DesignationFormValues } from '../types/designation';
import { toastService } from '../../../services/toastService';

const defaultFormValues: DesignationFormValues = {
  name: '',
  code: '',
  sortOrder: 0,
  isActive: true,
};

export default function DesignationList() {
  const dispatch = useAppDispatch();
  const { designations, loading, error } = useAppSelector((state) => state.designations);
  const [searchText, setSearchText] = useState('');
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingDesignation, setEditingDesignation] = useState<Designation | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Designation | null>(null);

  const [formValues, setFormValues] = useState<DesignationFormValues>(defaultFormValues);
  const [formError, setFormError] = useState('');

  useEffect(() => {
    void dispatch(fetchDesignations());
  }, [dispatch]);

  const filtered = useMemo(
    () =>
      designations.filter((d) => {
        const q = searchText.toLowerCase();
        return !q || d.name.toLowerCase().includes(q) || d.code.toLowerCase().includes(q);
      }),
    [designations, searchText],
  );

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

  const handleOpenAdd = () => {
    setEditingDesignation(null);
    setFormValues(defaultFormValues);
    setFormError('');
    setDialogOpen(true);
  };

  const handleOpenEdit = (d: Designation) => {
    setEditingDesignation(d);
    setFormValues({ name: d.name, code: d.code, sortOrder: d.sortOrder, isActive: d.isActive });
    setFormError('');
    setDialogOpen(true);
  };

  const handleSave = async () => {
    if (!formValues.name.trim()) {
      setFormError('Name is required');
      return;
    }
    setFormError('');
    try {
      if (editingDesignation) {
        await dispatch(updateDesignation({ id: editingDesignation.id, values: formValues })).unwrap();
        toastService.success('Designation updated successfully');
      } else {
        await dispatch(createDesignation(formValues)).unwrap();
        toastService.success('Designation created successfully');
      }
      setDialogOpen(false);
    } catch (err: any) {
      setFormError(err?.message || 'Failed to save designation');
    }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await dispatch(deleteDesignation(deleteTarget.id)).unwrap();
      setDeleteTarget(null);
      toastService.success('Designation deleted successfully');
    } catch {
      toastService.error('Failed to delete designation');
    }
  };

  return (
    <PageContainer title="Designation Master">
      <Stack spacing={3}>
        <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
          <Box>
            <Typography color="text.secondary" variant="body2">
              {filtered.length} of {designations.length} designations
            </Typography>
          </Box>
          <Button onClick={handleOpenAdd} startIcon={<AddIcon />} variant="contained">
            Add Designation
          </Button>
        </Stack>

        <TextField
          fullWidth
          label="Search designations"
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

        {error ? <Alert severity="error">{error}</Alert> : null}

        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell sx={{ fontWeight: 700 }}>Code</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Name</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Sort Order</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Status</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {loading ? (
                  <TableRow>
                    <TableCell align="center" colSpan={5}>
                      <Typography color="text.secondary" py={3}>
                        Loading...
                      </Typography>
                    </TableCell>
                  </TableRow>
                ) : paginated.length === 0 ? (
                  <TableRow>
                    <TableCell align="center" colSpan={5}>
                      <Typography color="text.secondary" py={3}>
                        No designations found
                      </Typography>
                    </TableCell>
                  </TableRow>
                ) : (
                  paginated.map((d) => (
                    <TableRow key={d.id} hover>
                      <TableCell>{d.code || '-'}</TableCell>
                      <TableCell>{d.name}</TableCell>
                      <TableCell>{d.sortOrder}</TableCell>
                      <TableCell>
                        <Chip
                          color={d.isActive ? 'success' : 'default'}
                          label={d.isActive ? 'Active' : 'Inactive'}
                          size="small"
                          variant="outlined"
                        />
                      </TableCell>
                      <TableCell>
                        <IconButton color="primary" onClick={() => handleOpenEdit(d)} size="small">
                          <EditIcon fontSize="small" />
                        </IconButton>
                        <IconButton color="error" onClick={() => setDeleteTarget(d)} size="small">
                          <DeleteIcon fontSize="small" />
                        </IconButton>
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

      <Dialog fullWidth maxWidth="sm" onClose={() => setDialogOpen(false)} open={dialogOpen}>
        <DialogTitle>{editingDesignation ? 'Edit Designation' : 'Add Designation'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            {formError && <Alert severity="error">{formError}</Alert>}
            <TextField
              fullWidth
              label="Name"
              onChange={(e) => setFormValues({ ...formValues, name: e.target.value })}
              required
              value={formValues.name}
            />
            <TextField
              fullWidth
              label="Code"
              onChange={(e) => setFormValues({ ...formValues, code: e.target.value })}
              required
              value={formValues.code}
            />
            <TextField
              fullWidth
              label="Sort Order"
              onChange={(e) => setFormValues({ ...formValues, sortOrder: Number(e.target.value) })}
              type="number"
              value={formValues.sortOrder}
            />
            <Stack alignItems="center" direction="row" spacing={1}>
              <Typography variant="body2">Active:</Typography>
              <input
                checked={formValues.isActive ?? true}
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
        <DialogTitle>Delete Designation?</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete <strong>{deleteTarget?.name}</strong>?
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
