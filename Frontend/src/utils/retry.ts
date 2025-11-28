/**
 * Retry utility with exponential backoff for failed API requests
 */

interface RetryOptions {
  maxRetries?: number;
  initialDelay?: number;
  maxDelay?: number;
  backoffMultiplier?: number;
  retryableStatuses?: number[];
}

const DEFAULT_OPTIONS: Required<RetryOptions> = {
  maxRetries: 3,
  initialDelay: 1000,
  maxDelay: 10000,
  backoffMultiplier: 2,
  retryableStatuses: [408, 429, 500, 502, 503, 504],
};

/**
 * Delay execution for specified milliseconds
 */
function delay(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

/**
 * Calculate exponential backoff delay
 */
function calculateDelay(
  attempt: number,
  initialDelay: number,
  maxDelay: number,
  multiplier: number
): number {
  const exponentialDelay = initialDelay * Math.pow(multiplier, attempt);
  const jitter = Math.random() * 0.1 * exponentialDelay; // Add 10% jitter
  return Math.min(exponentialDelay + jitter, maxDelay);
}

/**
 * Retry a function with exponential backoff
 */
export async function retryWithBackoff<T>(
  fn: () => Promise<T>,
  options: RetryOptions = {}
): Promise<T> {
  const opts = { ...DEFAULT_OPTIONS, ...options };
  let lastError: Error;

  for (let attempt = 0; attempt < opts.maxRetries; attempt++) {
    try {
      return await fn();
    } catch (error) {
      lastError = error as Error;

      // Check if error is retryable
      const isRetryable =
        error instanceof Error &&
        'status' in error &&
        typeof error.status === 'number' &&
        opts.retryableStatuses.includes(error.status);

      // Don't retry if not retryable or on last attempt
      if (!isRetryable || attempt === opts.maxRetries - 1) {
        throw error;
      }

      // Calculate delay and wait before retrying
      const delayMs = calculateDelay(
        attempt,
        opts.initialDelay,
        opts.maxDelay,
        opts.backoffMultiplier
      );

      console.warn(
        `Request failed (attempt ${attempt + 1}/${opts.maxRetries}). Retrying in ${Math.round(delayMs)}ms...`,
        error
      );

      await delay(delayMs);
    }
  }

  throw lastError!;
}

/**
 * Retry wrapper specifically for fetch requests
 */
export async function retryFetch(
  input: RequestInfo | URL,
  init?: RequestInit,
  options?: RetryOptions
): Promise<Response> {
  return retryWithBackoff(async () => {
    const response = await fetch(input, init);

    // Throw error for non-OK responses so retry logic can handle them
    if (!response.ok) {
      const error = new Error(`HTTP ${response.status}: ${response.statusText}`) as Error & {
        status: number;
      };
      error.status = response.status;
      throw error;
    }

    return response;
  }, options);
}
