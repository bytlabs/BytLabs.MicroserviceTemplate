import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';
import { fileURLToPath, URL } from 'node:url';

// Client-only SPA served by the .NET API under /console (base). In dev, `npm run dev` runs the Vite
// dev server and proxies the API endpoints so the console works against a locally-running API.
export default defineConfig({
  base: '/console/',
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  build: {
    outDir: 'dist',
    emptyOutDir: true,
  },
  server: {
    port: 5173,
    proxy: {
      '/graphql': 'http://localhost:5024',
      '/console/config': 'http://localhost:5024',
      '/auth': 'http://localhost:5024',
    },
  },
});
