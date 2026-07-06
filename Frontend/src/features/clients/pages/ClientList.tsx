import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import RefreshIcon from '@mui/icons-material/Refresh';
import SearchIcon from '@mui/icons-material/Search';
import VisibilityIcon from '@mui/icons-material/Visibility';
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
  MenuItem,
  Paper,
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
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { useEffect, useMemo, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
import PageContainer from '../../../components/common/PageContainer';
import {
  createClient,
  deleteClient,
  fetchClients,
  fetchStatuses,
  updateClient,
} from '../../../redux/slices/clientSlice';
import type { Client, CreateClientRequest, UpdateClientRequest } from '../types/client';
import { toastService } from '../../../services/toastService';
import { format, parseISO } from 'date-fns';

function formatDate(value: string | null | undefined): string {
  if (!value) return '-';
  try {
    return format(parseISO(value), 'dd-MMM-yyyy');
  } catch {
    return '-';
  }
}

type SortField = 'name' | 'contractStartDate' | 'contractEndDate' | 'statusName' | 'location' | 'createdOn';
type SortDir = 'asc' | 'desc';

const defaultCreate: CreateClientRequest = {
  name: '',
  contractStartDate: null,
  contractEndDate: null,
  statusId: '',
  location: '',
};

export default function ClientList() {
  const dispatch = useAppDispatch();
  const { clients, loading, error } = useAppSelector((state) => state.clients);
  const statuses = useAppSelector((state) => state.clients.statuses);

  const [searchText, setSearchText] = useState('');
  const [statusFilter, setStatusFilter] = useState('');

  const [sortField, setSortField] = useState<SortField>('name');
  const [sortDir, setSortDir] = useState<SortDir>('asc');

  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);

  const [dialogMode, setDialogMode] = useState<'add' | 'edit' | 'view' | null>(null);
  const [selectedClient, setSelectedClient] = useState<Client | null>(null);
  const [formValues, setFormValues] = useState<CreateClientRequest>(defaultCreate);
  const [formError, setFormError] = useState('');
  const [deleteTarget, setDeleteTarget] = useState<Client | null>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    dispatch(fetchClients());
    if (statuses.length === 0) {
      dispatch(fetchStatuses());
    }
  }, [dispatch, statuses.length]);

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
    let list = clients;
    if (q) {
      list = list.filter((c) => c.name.toLowerCase().includes(q));
    }
    if (statusFilter) {
      list = list.filter((c) => c.statusId === statusFilter);
    }
    list = [...list].sort((a, b) => {
      const aVal = String(a[sortField] ?? '');
      const bVal = String(b[sortField] ?? '');
      const cmp = aVal.localeCompare(bVal);
      return sortDir === 'asc' ? cmp : -cmp;
    });
    return list;
  }, [clients, searchText, statusFilter, sortField, sortDir]);

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
    dispatch(fetchClients());
  };

  const handleOpenAdd = () => {
    setSelectedClient(null);
    setFormValues(defaultCreate);
    setFormError('');
    setDialogMode('add');
  };

  const handleOpenEdit = (client: Client) => {
    setSelectedClient(client);
    setFormValues({
      name: client.name,
      contractStartDate: client.contractStartDate,
      contractEndDate: client.contractEndDate,
      statusId: client.statusId,
      location: client.location ?? '',
    });
    setFormError('');
    setDialogMode('edit');
  };

  const handleOpenView = (client: Client) => {
    setSelectedClient(client);
    setDialogMode('view');
  };

  const handleCloseDialog = () => {
    setDialogMode(null);
    setSelectedClient(null);
    setFormError('');
  };

  const validateForm = (): boolean => {
    const trimmed = formValues.name.trim();
    if (!trimmed) {
      setFormError('Client Name is required');
      return false;
    }
    if (trimmed.length > 200) {
      setFormError('Client Name must not exceed 200 characters');
      return false;
    }
    if (!formValues.contractStartDate) {
      setFormError('Contract Start Date is required');
      return false;
    }
    if (!formValues.contractEndDate) {
      setFormError('Contract End Date is required');
      return false;
    }
    if (formValues.contractEndDate < formValues.contractStartDate) {
      setFormError('Contract End Date cannot be earlier than Contract Start Date');
      return false;
    }
    if (!formValues.statusId) {
      setFormError('Status is required');
      return false;
    }
    if (!formValues.location.trim()) {
      setFormError('Location is required');
      return false;
    }
    return true;
  };

  const handleSave = async () => {
    if (!validateForm()) return;
    setSaving(true);
    setFormError('');
    try {
      const values: CreateClientRequest = {
        name: formValues.name.trim(),
        contractStartDate: formValues.contractStartDate,
        contractEndDate: formValues.contractEndDate,
        statusId: formValues.statusId,
        location: formValues.location.trim(),
      };

      if (dialogMode === 'edit' && selectedClient) {
        const updateValues: UpdateClientRequest = {
          ...values,
          rowVersion: selectedClient.rowVersion,
        };
        await dispatch(updateClient({ id: selectedClient.id, values: updateValues })).unwrap();
        toastService.success('Client updated successfully');
      } else {
        await dispatch(createClient(values)).unwrap();
        toastService.success('Client created successfully');
      }
      handleCloseDialog();
    } catch (err: any) {
      setFormError(err?.message || 'Failed to save client');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      await dispatch(deleteClient(deleteTarget.id)).unwrap();
      setDeleteTarget(null);
      toastService.success('Client deleted successfully');
    } catch {
      toastService.error('Failed to delete client');
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
    <PageContainer title="Client Management">
      <Stack spacing={3}>
        <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
          <Box>
            <Typography color="text.secondary" variant="body2">
              {filtered.length} of {clients.length} clients
            </Typography>
          </Box>
          <Stack direction="row" spacing={1.5}>
            <Tooltip title="Refresh">
              <Button onClick={handleRefresh} startIcon={<RefreshIcon />} variant="outlined">
                Refresh
              </Button>
            </Tooltip>
            <Button onClick={handleOpenAdd} startIcon={<AddIcon />} variant="contained">
              Add Client
            </Button>
          </Stack>
        </Stack>

        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
          <TextField
            fullWidth
            label="Search by client name"
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
          <TextField
            label="Filter by Status"
            onChange={(e) => {
              setStatusFilter(e.target.value);
              setPage(0);
            }}
            select
            slotProps={{ select: { displayEmpty: true } }}
            sx={{ minWidth: 200 }}
            value={statusFilter}
          >
            <MenuItem value="">All Statuses</MenuItem>
            {statuses.map((s) => (
              <MenuItem key={s.id} value={s.id}>
                {s.name}
              </MenuItem>
            ))}
          </TextField>
        </Stack>

        {error ? <Alert severity="error">{error}</Alert> : null}

        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <SortableHeader field="name" label="Client Name" />
                  <SortableHeader field="contractStartDate" label="Contract Start Date" />
                  <SortableHeader field="contractEndDate" label="Contract End Date" />
                  <SortableHeader field="statusName" label="Status" />
                  <SortableHeader field="location" label="Location" />
                  <SortableHeader field="createdOn" label="Created On" />
                  <TableCell sx={{ fontWeight: 700 }}>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {loading ? (
                  <TableRow>
                    <TableCell align="center" colSpan={7}>
                      <Typography color="text.secondary" py={3}>
                        Loading...
                      </Typography>
                    </TableCell>
                  </TableRow>
                ) : paginated.length === 0 ? (
                  <TableRow>
                    <TableCell align="center" colSpan={7}>
                      <Typography color="text.secondary" py={3}>
                        No clients found
                      </Typography>
                    </TableCell>
                  </TableRow>
                ) : (
                  paginated.map((client) => (
                    <TableRow key={client.id} hover>
                      <TableCell sx={{ fontWeight: 600 }}>{client.name}</TableCell>
                      <TableCell>{formatDate(client.contractStartDate)}</TableCell>
                      <TableCell>{formatDate(client.contractEndDate)}</TableCell>
                      <TableCell>
                        <Chip color="primary" label={client.statusName || '-'} size="small" variant="outlined" />
                      </TableCell>
                      <TableCell>{client.location || '-'}</TableCell>
                      <TableCell>{formatDate(client.createdOn)}</TableCell>
                      <TableCell>
                        <Stack direction="row" spacing={0.5}>
                          <Tooltip title="View">
                            <IconButton color="info" onClick={() => handleOpenView(client)} size="small">
                              <VisibilityIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                          <Tooltip title="Edit">
                            <IconButton color="primary" onClick={() => handleOpenEdit(client)} size="small">
                              <EditIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                          <Tooltip title="Delete">
                            <IconButton color="error" onClick={() => setDeleteTarget(client)} size="small">
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
      <Dialog fullWidth maxWidth="sm" onClose={handleCloseDialog} open={dialogMode === 'add' || dialogMode === 'edit'}>
        <DialogTitle>{dialogMode === 'edit' ? 'Edit Client' : 'Add Client'}</DialogTitle>
        <DialogContent>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <Stack spacing={2.5} sx={{ mt: 1 }}>
              {formError && <Alert severity="error">{formError}</Alert>}
              <TextField
                fullWidth
                label="Client Name"
                onChange={(e) => setFormValues({ ...formValues, name: e.target.value })}
                required
                value={formValues.name}
              />
              <DatePicker
                label="Contract Start Date"
                onChange={(date) => setFormValues({ ...formValues, contractStartDate: date ? date.toISOString() : null })}
                slotProps={{ textField: { fullWidth: true, required: true } }}
                value={formValues.contractStartDate ? parseISO(formValues.contractStartDate) : null}
              />
              <DatePicker
                label="Contract End Date"
                onChange={(date) => setFormValues({ ...formValues, contractEndDate: date ? date.toISOString() : null })}
                slotProps={{ textField: { fullWidth: true, required: true } }}
                value={formValues.contractEndDate ? parseISO(formValues.contractEndDate) : null}
              />
              <TextField
                fullWidth
                label="Status"
                onChange={(e) => setFormValues({ ...formValues, statusId: e.target.value })}
                required
                select
                value={formValues.statusId}
              >
                <MenuItem value="" disabled>
                  Select Status
                </MenuItem>
                {statuses.map((s) => (
                  <MenuItem key={s.id} value={s.id}>
                    {s.name}
                  </MenuItem>
                ))}
              </TextField>
              <TextField
                fullWidth
                label="Location"
                onChange={(e) => setFormValues({ ...formValues, location: e.target.value })}
                required
                value={formValues.location}
              />
            </Stack>
          </LocalizationProvider>
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

      {/* View Dialog */}
      <Dialog fullWidth maxWidth="sm" onClose={handleCloseDialog} open={dialogMode === 'view'}>
        <DialogTitle>Client Details</DialogTitle>
        <DialogContent>
          {selectedClient && (
            <Stack spacing={2} sx={{ mt: 1 }}>
              <Box>
                <Typography color="text.secondary" variant="caption">
                  Client Name
                </Typography>
                <Typography fontWeight={600}>{selectedClient.name}</Typography>
              </Box>
              <Box>
                <Typography color="text.secondary" variant="caption">
                  Contract Start Date
                </Typography>
                <Typography>{formatDate(selectedClient.contractStartDate)}</Typography>
              </Box>
              <Box>
                <Typography color="text.secondary" variant="caption">
                  Contract End Date
                </Typography>
                <Typography>{formatDate(selectedClient.contractEndDate)}</Typography>
              </Box>
              <Box>
                <Typography color="text.secondary" variant="caption">
                  Status
                </Typography>
                <Typography>{selectedClient.statusName || '-'}</Typography>
              </Box>
              <Box>
                <Typography color="text.secondary" variant="caption">
                  Location
                </Typography>
                <Typography>{selectedClient.location || '-'}</Typography>
              </Box>
              <Box>
                <Typography color="text.secondary" variant="caption">
                  Created On
                </Typography>
                <Typography>{formatDate(selectedClient.createdOn)}</Typography>
              </Box>
              <Box>
                <Typography color="text.secondary" variant="caption">
                  Modified On
                </Typography>
                <Typography>{formatDate(selectedClient.modifiedOn)}</Typography>
              </Box>
            </Stack>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Close</Button>
        </DialogActions>
      </Dialog>

      {/* Delete Confirmation */}
      <Dialog onClose={() => setDeleteTarget(null)} open={Boolean(deleteTarget)}>
        <DialogTitle>Delete Client?</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete <strong>{deleteTarget?.name}</strong>?
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
