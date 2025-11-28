import { describe, it, expect, vi, beforeEach } from 'vitest';
import { retryWithBackoff, retryFetch } from './retry';

describe('retryWithBackoff', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should return result on first success', async () => {
    const fn = vi.fn().mockResolvedValue('success');
    const result = await retryWithBackoff(fn);
    
    expect(result).toBe('success');
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it('should retry on failure and eventually succeed', async () => {
    const fn = vi.fn()
      .mockRejectedValueOnce(Object.assign(new Error('Network error'), { status: 503 }))
      .mockResolvedValue('success');
    
    const result = await retryWithBackoff(fn, { maxRetries: 3, initialDelay: 10 });
    
    expect(result).toBe('success');
    expect(fn).toHaveBeenCalledTimes(2);
  });

  it('should throw error after max retries', async () => {
    const error = Object.assign(new Error('Network error'), { status: 503 });
    const fn = vi.fn().mockRejectedValue(error);
    
    await expect(
      retryWithBackoff(fn, { maxRetries: 2, initialDelay: 10 })
    ).rejects.toThrow('Network error');
    
    expect(fn).toHaveBeenCalledTimes(2);
  });

  it('should not retry non-retryable errors', async () => {
    const error = Object.assign(new Error('Bad request'), { status: 400 });
    const fn = vi.fn().mockRejectedValue(error);
    
    await expect(
      retryWithBackoff(fn, { maxRetries: 3, initialDelay: 10 })
    ).rejects.toThrow('Bad request');
    
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it('should retry specific status codes', async () => {
    const retryableStatuses = [408, 429, 500, 502, 503, 504];
    
    for (const status of retryableStatuses) {
      const error = Object.assign(new Error(`Error ${status}`), { status });
      const fn = vi.fn()
        .mockRejectedValueOnce(error)
        .mockResolvedValue('success');
      
      await retryWithBackoff(fn, { maxRetries: 2, initialDelay: 10 });
      expect(fn).toHaveBeenCalledTimes(2);
    }
  });

  it('should respect custom retry options', async () => {
    const fn = vi.fn()
      .mockRejectedValueOnce(Object.assign(new Error('Error'), { status: 503 }))
      .mockRejectedValueOnce(Object.assign(new Error('Error'), { status: 503 }))
      .mockResolvedValue('success');
    
    const result = await retryWithBackoff(fn, {
      maxRetries: 5,
      initialDelay: 5,
      maxDelay: 100,
      backoffMultiplier: 2,
    });
    
    expect(result).toBe('success');
    expect(fn).toHaveBeenCalledTimes(3);
  });
});

describe('retryFetch', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should return successful response', async () => {
    const mockResponse = {
      ok: true,
      status: 200,
      statusText: 'OK',
    } as Response;
    
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(mockResponse));
    
    const result = await retryFetch('https://api.example.com/data', {}, { maxRetries: 2, initialDelay: 10 });
    
    expect(result).toBe(mockResponse);
    expect(fetch).toHaveBeenCalledTimes(1);
  });

  it('should retry on 503 error', async () => {
    const errorResponse = {
      ok: false,
      status: 503,
      statusText: 'Service Unavailable',
    } as Response;
    
    const successResponse = {
      ok: true,
      status: 200,
      statusText: 'OK',
    } as Response;
    
    vi.stubGlobal('fetch', vi.fn()
      .mockResolvedValueOnce(errorResponse)
      .mockResolvedValue(successResponse));
    
    const result = await retryFetch('https://api.example.com/data', {}, { maxRetries: 2, initialDelay: 10 });
    
    expect(result).toBe(successResponse);
    expect(fetch).toHaveBeenCalledTimes(2);
  });

  it('should throw error for non-retryable status', async () => {
    const errorResponse = {
      ok: false,
      status: 400,
      statusText: 'Bad Request',
    } as Response;
    
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(errorResponse));
    
    await expect(
      retryFetch('https://api.example.com/data', {}, { maxRetries: 3, initialDelay: 10 })
    ).rejects.toThrow('HTTP 400');
    
    expect(fetch).toHaveBeenCalledTimes(1);
  });
});
