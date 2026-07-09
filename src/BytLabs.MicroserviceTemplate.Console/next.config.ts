import type { NextConfig } from 'next';

// Static export served by the .NET API under /console (single port, SPA-only).
const nextConfig: NextConfig = {
  output: 'export',
  basePath: '/console',
  trailingSlash: true,
  images: { unoptimized: true },
  distDir: 'out',
};

export default nextConfig;
