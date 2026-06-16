import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:7132',
        changeOrigin: true,
        secure: false,
      },
    },
    watch: {
      ignored: ['**/.vs/**', '**/backend/**', '**/node_modules/**'],
    },
  },
})
