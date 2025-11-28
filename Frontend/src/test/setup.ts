import '@testing-library/jest-dom';
import { cleanup } from '@testing-library/react';
import { afterEach } from 'vitest';

// Cleanup after each test
afterEach(() => {
  cleanup();
});

// Mock localStorage
const localStorageMock = {
  getItem: vi.fn(),
  setItem: vi.fn(),
  removeItem: vi.fn(),
  clear: vi.fn(),
};
global.localStorage = localStorageMock as Storage;

// Mock fetch
global.fetch = vi.fn();

// Mock window.location
delete (window as { location?: Location }).location;
window.location = { href: '', reload: vi.fn() } as unknown as Location;
