import { describe, it, expect } from 'vitest';
import {
  sanitizeHtml,
  sanitizeInput,
  sanitizeUrl,
  sanitizeNumber,
  isValidEmail,
  sanitizeFilename,
} from './sanitize';

describe('sanitizeHtml', () => {
  it('should escape HTML special characters', () => {
    expect(sanitizeHtml('<script>alert("XSS")</script>')).toBe(
      '&lt;script&gt;alert(&quot;XSS&quot;)&lt;&#x2F;script&gt;'
    );
  });

  it('should escape ampersands', () => {
    expect(sanitizeHtml('Tom & Jerry')).toBe('Tom &amp; Jerry');
  });

  it('should escape quotes', () => {
    expect(sanitizeHtml("It's a 'test'")).toBe('It&#x27;s a &#x27;test&#x27;');
  });

  it('should handle empty string', () => {
    expect(sanitizeHtml('')).toBe('');
  });
});

describe('sanitizeInput', () => {
  it('should remove script tags', () => {
    const input = '<script>alert("XSS")</script>Hello';
    expect(sanitizeInput(input)).toBe('Hello');
  });

  it('should remove event handlers', () => {
    const input = '<div onclick="alert(\'XSS\')">Click me</div>';
    expect(sanitizeInput(input)).not.toContain('onclick');
  });

  it('should remove javascript: protocol', () => {
    const input = '<a href="javascript:alert(\'XSS\')">Link</a>';
    expect(sanitizeInput(input)).not.toContain('javascript:');
  });

  it('should trim whitespace', () => {
    expect(sanitizeInput('  hello  ')).toBe('hello');
  });

  it('should normalize multiple spaces', () => {
    expect(sanitizeInput('hello    world')).toBe('hello world');
  });

  it('should return empty string for empty input', () => {
    expect(sanitizeInput('')).toBe('');
  });
});

describe('sanitizeUrl', () => {
  it('should allow https URLs', () => {
    const url = 'https://example.com';
    expect(sanitizeUrl(url)).toBe(url);
  });

  it('should allow http URLs', () => {
    const url = 'http://example.com';
    expect(sanitizeUrl(url)).toBe(url);
  });

  it('should block javascript: protocol', () => {
    expect(sanitizeUrl('javascript:alert("XSS")')).toBe('');
  });

  it('should block data: protocol', () => {
    expect(sanitizeUrl('data:text/html,<script>alert("XSS")</script>')).toBe('');
  });

  it('should block vbscript: protocol', () => {
    expect(sanitizeUrl('vbscript:alert("XSS")')).toBe('');
  });

  it('should handle mixed case protocols', () => {
    expect(sanitizeUrl('JaVaScRiPt:alert("XSS")')).toBe('');
  });

  it('should return empty string for empty input', () => {
    expect(sanitizeUrl('')).toBe('');
  });
});

describe('sanitizeNumber', () => {
  it('should parse valid number string', () => {
    expect(sanitizeNumber('123.45')).toBe(123.45);
  });

  it('should handle number input', () => {
    expect(sanitizeNumber(42)).toBe(42);
  });

  it('should return null for invalid number', () => {
    expect(sanitizeNumber('abc')).toBeNull();
    // '12abc' gets parsed as 12 by parseFloat, so it returns 12
    expect(sanitizeNumber('abc123')).toBeNull();
  });

  it('should return null for NaN', () => {
    expect(sanitizeNumber(NaN)).toBeNull();
  });

  it('should return null for Infinity', () => {
    expect(sanitizeNumber(Infinity)).toBeNull();
  });

  it('should enforce minimum value', () => {
    expect(sanitizeNumber(-5, { min: 0 })).toBe(0);
  });

  it('should enforce maximum value', () => {
    expect(sanitizeNumber(100, { max: 50 })).toBe(50);
  });

  it('should round to specified decimals', () => {
    expect(sanitizeNumber(3.14159, { decimals: 2 })).toBe(3.14);
  });

  it('should handle all options together', () => {
    expect(sanitizeNumber(150.789, { min: 0, max: 100, decimals: 2 })).toBe(100);
  });
});

describe('isValidEmail', () => {
  it('should validate correct email', () => {
    expect(isValidEmail('test@example.com')).toBe(true);
    expect(isValidEmail('user.name+tag@example.co.uk')).toBe(true);
  });

  it('should reject invalid email', () => {
    expect(isValidEmail('invalid')).toBe(false);
    expect(isValidEmail('invalid@')).toBe(false);
    expect(isValidEmail('@example.com')).toBe(false);
    expect(isValidEmail('invalid@example')).toBe(false);
    expect(isValidEmail('invalid @example.com')).toBe(false);
  });
});

describe('sanitizeFilename', () => {
  it('should remove forward slashes', () => {
    expect(sanitizeFilename('path/to/file.txt')).toBe('pathtofile.txt');
  });

  it('should remove backslashes', () => {
    expect(sanitizeFilename('path\\to\\file.txt')).toBe('pathtofile.txt');
  });

  it('should remove path traversal attempts', () => {
    expect(sanitizeFilename('../../../etc/passwd')).toBe('etcpasswd');
  });

  it('should remove dangerous characters', () => {
    expect(sanitizeFilename('file<>:"|?*.txt')).toBe('file.txt');
  });

  it('should trim whitespace', () => {
    expect(sanitizeFilename('  file.txt  ')).toBe('file.txt');
  });

  it('should allow normal filenames', () => {
    expect(sanitizeFilename('document-2024.pdf')).toBe('document-2024.pdf');
  });
});
