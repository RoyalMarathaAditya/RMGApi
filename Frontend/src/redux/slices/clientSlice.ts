import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { clientService } from '../../features/clients/services/clientService';
import type { Client, CreateClientRequest, UpdateClientRequest } from '../../features/clients/types/client';

interface ClientState {
  clients: Client[];
  statuses: Array<{ id: string; name: string }>;
  loading: boolean;
  error: string | null;
}

const initialState: ClientState = {
  clients: [],
  statuses: [],
  loading: false,
  error: null,
};

export const fetchClients = createAsyncThunk('clients/fetchClients', clientService.getAll);

export const fetchStatuses = createAsyncThunk('clients/fetchStatuses', clientService.getStatuses);

export const createClient = createAsyncThunk(
  'clients/createClient',
  async (values: CreateClientRequest) => clientService.create(values),
);

export const updateClient = createAsyncThunk(
  'clients/updateClient',
  async ({ id, values }: { id: number; values: UpdateClientRequest }) =>
    clientService.update(id, values),
);

export const deleteClient = createAsyncThunk(
  'clients/deleteClient',
  async (id: number) => {
    await clientService.delete(id);
    return id;
  },
);

const clientSlice = createSlice({
  name: 'clients',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchClients.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchClients.fulfilled, (state, action) => {
        state.loading = false;
        state.clients = action.payload;
      })
      .addCase(fetchClients.rejected, (state) => {
        state.loading = false;
        state.error = 'Unable to load clients.';
      })
      .addCase(fetchStatuses.fulfilled, (state, action) => {
        state.statuses = action.payload;
      })
      .addCase(createClient.fulfilled, (state, action) => {
        state.clients.unshift(action.payload);
      })
      .addCase(updateClient.fulfilled, (state, action) => {
        state.clients = state.clients.map((c) =>
          c.id === action.payload.id ? action.payload : c,
        );
      })
      .addCase(deleteClient.fulfilled, (state, action) => {
        state.clients = state.clients.filter((c) => c.id !== action.payload);
      });
  },
});

export default clientSlice.reducer;
