/**
 * Sanitization utilities to prevent XSS attacks
 */

/**
 * Sanitize HTML string by escaping special characters
 */
export function sanitizeHtml(text: string): string {
  const map: Record<string, string> = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#x27;',
    '/': '&#x2F;',
  };
  
  return text.replace(/[&<>"'/]/g, (char) => map[char]);
}

/**
 * Sanitize user input for safe display
 * Removes potentially dangerous characters and patterns
 */
export function sanitizeInput(input: string): string {
  if (!input) return '';
  
  // Remove any script tags or event handlers
  let sanitized = input
    .replace(/<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, '')
    .replace(/on\w+\s*=\s*["'][^"']*["']/gi, '')
    .replace(/javascript:/gi, '');
  
  // Trim and normalize whitespace
  sanitized = sanitized.trim().replace(/\s+/g, ' ');
  
  return sanitized;
}

/**
 * Sanitize URL to prevent javascript: and data: URI XSS
 */
export function sanitizeUrl(url: string): string {
  if (!url) return '';
  
  const trimmed = url.trim().toLowerCase();
  
  // Block dangerous protocols
  if (
    trimmed.startsWith('javascript:') ||
    trimmed.startsWith('data:') ||
    trimmed.startsWith('vbscript:')
  ) {
    return '';
  }
  
  return url;
}

/**
 * Validate and sanitize numeric input
 */
export function sanitizeNumber(value: string | number, options?: {
  min?: number;
  max?: number;
  decimals?: number;
}): number | null {
  const num = typeof value === 'string' ? parseFloat(value) : value;
  
  if (isNaN(num) || !isFinite(num)) {
    return null;
  }
  
  let sanitized = num;
  
  if (options?.min !== undefined && sanitized < options.min) {
    sanitized = options.min;
  }
  
  if (options?.max !== undefined && sanitized > options.max) {
    sanitized = options.max;
  }
  
  if (options?.decimals !== undefined) {
    sanitized = parseFloat(sanitized.toFixed(options.decimals));
  }
  
  return sanitized;
}

/**
 * Validate email format
 */
export function isValidEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}

/**
 * Sanitize filename for safe file operations
 */
export function sanitizeFilename(filename: string): string {
  // Remove path traversal attempts and dangerous characters
  return filename
    .replace(/[/\\]/g, '')
    .replace(/\.\./g, '')
    // eslint-disable-next-line no-control-regex
    .replace(/[<>:"|?*\u0000-\u001F]/g, '')
    .trim();
}
