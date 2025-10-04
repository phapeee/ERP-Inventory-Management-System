// playwright.config.ts
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: [['html', { open: 'never' }], ['list']],
  use: {
    // If you use Angular CLI dev server:
    baseURL: 'http://127.0.0.1:4200',
    // If you use Vite/Analog instead, change to:
    // baseURL: 'http://127.0.0.1:5173',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
    video: 'retain-on-failure'
  },

  projects: [
    { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
    { name: 'firefox',  use: { ...devices['Desktop Firefox'] } },
    { name: 'webkit',   use: { ...devices['Desktop Safari'] } },
  ],

  // ---------- Start your web app before tests ----------
  // Use ONE of these webServer configs:

  // A) Angular CLI dev server (ng serve)
  webServer: {
    command: 'npm run start -- --port=4200 --host 127.0.0.1',
    url: 'http://127.0.0.1:4200',
    reuseExistingServer: !process.env.CI,
    timeout: 120_000
  },

  // B) Vite / Analog dev server
  // webServer: {
  //   command: 'npm run dev -- --port=5173 --host 127.0.0.1',
  //   url: 'http://127.0.0.1:5173',
  //   reuseExistingServer: !process.env.CI,
  //   timeout: 120_000
  // },
});
