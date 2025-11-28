import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { Toast } from './Toast';

type ToastType = 'success' | 'error' | 'info' | 'warning';

describe('Toast', () => {
  const mockOnClose = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should render success toast', () => {
    render(<Toast message="Success message" type="success" onClose={mockOnClose} />);
    
    expect(screen.getByText('Success message')).toBeInTheDocument();
    expect(screen.getByRole('alert')).toBeInTheDocument();
  });

  it('should render error toast', () => {
    render(<Toast message="Error message" type="error" onClose={mockOnClose} />);
    
    expect(screen.getByText('Error message')).toBeInTheDocument();
    expect(screen.getByRole('alert')).toBeInTheDocument();
  });

  it('should render info toast', () => {
    render(<Toast message="Info message" type="info" onClose={mockOnClose} />);
    
    expect(screen.getByText('Info message')).toBeInTheDocument();
    expect(screen.getByRole('alert')).toBeInTheDocument();
  });

  it('should render warning toast', () => {
    render(<Toast message="Warning message" type="warning" onClose={mockOnClose} />);
    
    expect(screen.getByText('Warning message')).toBeInTheDocument();
    expect(screen.getByRole('alert')).toBeInTheDocument();
  });

  it('should display correct icon for each type', () => {
    const types: ToastType[] = ['success', 'error', 'info', 'warning'];
    const expectedIcons = ['✓', '✕', 'ℹ', '⚠'];

    types.forEach((type, index) => {
      const { unmount } = render(<Toast message="Test" type={type} onClose={mockOnClose} />);
      expect(screen.getByText(expectedIcons[index])).toBeInTheDocument();
      unmount();
    });
  });

  it('should call onClose when close button is clicked', async () => {
    render(<Toast message="Test message" type="info" onClose={mockOnClose} />);
    
    const closeButton = screen.getByLabelText('Close notification');
    closeButton.click();

    await waitFor(() => {
      expect(mockOnClose).toHaveBeenCalledTimes(1);
    }, { timeout: 500 });
  });

  it('should become visible after mount', async () => {
    render(<Toast message="Test" type="success" onClose={mockOnClose} />);
    
    await waitFor(() => {
      expect(screen.getByRole('alert')).toBeInTheDocument();
    });
  });
});
