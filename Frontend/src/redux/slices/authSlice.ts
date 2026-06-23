import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import api from '../../services/api';
import type { AuthState, AuthUser, LoginResponse } from '../../features/auth/authTypes';

// Redux slice for auth state: login, logout, token refresh, and app startup initialization
const TOKEN_KEY = 'hrms_auth_token';
const REFRESH_TOKEN_KEY = 'hrms_auth_refresh_token';

const initialState: AuthState = {
  user: null,
  token: null,
  isAuthenticated: false,
  loading: true,
};

// Async thunk: initializes auth on app startup by checking localStorage tokens and calling /auth/me
export const initializeAuth = createAsyncThunk('auth/initialize', async (_, { rejectWithValue }) => {
  try {
    const storedToken = localStorage.getItem(TOKEN_KEY);
    const storedRefreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);

    if (!storedToken && !storedRefreshToken) {
      return rejectWithValue(null);
    }

    if (!storedToken && storedRefreshToken) {
      const refreshResponse = await api.post('/auth/refresh-token', { refreshToken: storedRefreshToken });
      localStorage.setItem(TOKEN_KEY, refreshResponse.data.token);
      localStorage.setItem(REFRESH_TOKEN_KEY, refreshResponse.data.refreshToken);
    }

    const response = await api.get<AuthUser>('/auth/me');
    return { user: response.data, token: localStorage.getItem(TOKEN_KEY)! };
  } catch {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    return rejectWithValue(null);
  }
});

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    login: (state, action: PayloadAction<LoginResponse>) => {
      state.user = action.payload.user;
      state.token = action.payload.token;
      state.isAuthenticated = true;
      state.loading = false;
    },
    logout: (state) => {
      state.user = null;
      state.token = null;
      state.isAuthenticated = false;
      state.loading = false;
    },
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.loading = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(initializeAuth.pending, (state) => {
        state.loading = true;
      })
      .addCase(initializeAuth.fulfilled, (state, action) => {
        state.user = action.payload.user;
        state.token = action.payload.token;
        state.isAuthenticated = true;
        state.loading = false;
      })
      .addCase(initializeAuth.rejected, (state) => {
        state.user = null;
        state.token = null;
        state.isAuthenticated = false;
        state.loading = false;
      });
  },
});

export const { login, logout, setLoading } = authSlice.actions;
export default authSlice.reducer;
