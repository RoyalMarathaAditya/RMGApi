import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import api from '../../services/api';
import type { AuthState, AuthUser, LoginResponse } from '../../features/auth/authTypes';

const TOKEN_KEY = 'hrms_auth_token';
const REFRESH_TOKEN_KEY = 'hrms_auth_refresh_token';

const initialState: AuthState = {
  user: null,
  token: null,
  isAuthenticated: false,
  loading: true,
  forcePasswordChange: false,
};

function getForcePasswordChangeFromToken(token: string): boolean {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.passwordResetRequired === 'true';
  } catch {
    return false;
  }
}

export const initializeAuth = createAsyncThunk('auth/initialize', async (_, { rejectWithValue }) => {
  try {
    let storedToken = localStorage.getItem(TOKEN_KEY);
    const storedRefreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);

    if (!storedToken && !storedRefreshToken) {
      return rejectWithValue(null);
    }

    if (!storedToken && storedRefreshToken) {
      const refreshResponse = await api.post('/auth/refresh-token', { refreshToken: storedRefreshToken });
      storedToken = refreshResponse.data.token;
      localStorage.setItem(TOKEN_KEY, storedToken);
      localStorage.setItem(REFRESH_TOKEN_KEY, refreshResponse.data.refreshToken);
    }

    const response = await api.get<AuthUser>('/auth/me');
    const forcePasswordChange = getForcePasswordChangeFromToken(storedToken!);
    return { user: response.data, token: storedToken!, forcePasswordChange };
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
      state.forcePasswordChange = action.payload.forcePasswordChange;
    },
    logout: (state) => {
      state.user = null;
      state.token = null;
      state.isAuthenticated = false;
      state.loading = false;
      state.forcePasswordChange = false;
      localStorage.removeItem(TOKEN_KEY);
      localStorage.removeItem(REFRESH_TOKEN_KEY);
    },
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.loading = action.payload;
    },
    clearForcePasswordChange: (state) => {
      state.forcePasswordChange = false;
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
        state.forcePasswordChange = action.payload.forcePasswordChange;
      })
      .addCase(initializeAuth.rejected, (state) => {
        state.user = null;
        state.token = null;
        state.isAuthenticated = false;
        state.loading = false;
      });
  },
});

export const { login, logout, setLoading, clearForcePasswordChange } = authSlice.actions;
export default authSlice.reducer;
