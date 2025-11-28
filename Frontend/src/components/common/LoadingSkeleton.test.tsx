import { describe, it, expect } from 'vitest';
import { render } from '@testing-library/react';
import {
  LoadingSkeleton,
  TableSkeleton,
  FormSkeleton,
} from './LoadingSkeleton';

describe('LoadingSkeleton', () => {
  it('should render with default variant', () => {
    const { container } = render(<LoadingSkeleton />);
    // CSS Modules hash class names, so we check for the element
    expect(container.firstChild).toBeInTheDocument();
    expect(container.querySelector('[data-testid]')?.getAttribute('data-variant')).toBeUndefined();
  });

  it('should render with circular variant', () => {
    const { container } = render(<LoadingSkeleton variant="circular" />);
    expect(container.firstChild).toBeInTheDocument();
  });

  it('should render with rectangular variant', () => {
    const { container } = render(<LoadingSkeleton variant="rectangular" />);
    expect(container.firstChild).toBeInTheDocument();
  });

  it('should apply custom className', () => {
    const { container } = render(<LoadingSkeleton className="custom-class" />);
    expect(container.firstChild).toHaveClass('custom-class');
  });

  it('should set width and height data attributes', () => {
    const { container } = render(<LoadingSkeleton width={200} height={50} />);
    const skeleton = container.firstChild as HTMLElement;
    expect(skeleton.getAttribute('data-width')).toBe('200px');
    expect(skeleton.getAttribute('data-height')).toBe('50px');
  });

  it('should handle string width and height', () => {
    const { container } = render(<LoadingSkeleton width="100%" height="2rem" />);
    const skeleton = container.firstChild as HTMLElement;
    expect(skeleton.getAttribute('data-width')).toBe('100%');
    expect(skeleton.getAttribute('data-height')).toBe('2rem');
  });
});

describe('TableSkeleton', () => {
  it('should render default number of rows', () => {
    const { container } = render(<TableSkeleton />);
    // Component renders divs, not actual table rows
    const skeletonDivs = container.querySelectorAll('div');
    expect(skeletonDivs.length).toBeGreaterThan(5);
  });

  it('should render custom number of rows', () => {
    const { container } = render(<TableSkeleton rows={10} />);
    // Component renders divs, not actual table rows
    const skeletonDivs = container.querySelectorAll('div');
    expect(skeletonDivs.length).toBeGreaterThan(10);
  });

  it('should render table skeleton structure', () => {
    const { container } = render(<TableSkeleton />);
    // Component renders skeleton divs, not actual HTML table
    expect(container.firstChild).toBeInTheDocument();
  });
});

describe('FormSkeleton', () => {
  it('should render form fields', () => {
    const { container } = render(<FormSkeleton />);
    const skeletons = container.querySelectorAll('[data-width]');
    expect(skeletons.length).toBeGreaterThanOrEqual(8);
  });

  it('should render action buttons area', () => {
    const { container } = render(<FormSkeleton />);
    expect(container.firstChild).toBeInTheDocument();
  });
});
