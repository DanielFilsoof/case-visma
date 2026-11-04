import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

const apiUrl = 'http://localhost:5080'

export default defineConfig({
  plugins: [react()],
  server: {
    // Forward API calls to the .NET backend during development (no CORS needed).
    proxy: {
      '/api': apiUrl,
    },
  },
  build: {
    outDir: '../../backend/src/TodoApp.Api/wwwroot',
    emptyOutDir: true,
  },
})
