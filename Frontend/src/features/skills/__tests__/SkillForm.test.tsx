import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import SkillForm from '../components/SkillForm';

describe('SkillForm', () => {
  it('renders create fields and submits valid values', async () => {
    const onSubmit = jest.fn();

    render(<SkillForm mode="create" onCancel={jest.fn()} onSubmit={onSubmit} />);

    fireEvent.change(screen.getByLabelText(/Skill Code/i), { target: { value: 'SKL-900' } });
    fireEvent.change(screen.getByLabelText(/Skill Name/i), { target: { value: 'Enterprise React' } });
    fireEvent.change(screen.getByLabelText(/Description/i), { target: { value: 'Production React architecture' } });
    fireEvent.click(screen.getByRole('button', { name: /Save/i }));

    await waitFor(() => expect(onSubmit).toHaveBeenCalled());
  });

  it('shows validation when required values are missing', async () => {
    render(<SkillForm mode="create" onCancel={jest.fn()} onSubmit={jest.fn()} />);

    fireEvent.click(screen.getByRole('button', { name: /Save/i }));

    expect(await screen.findByText(/Skill code is required/i)).toBeInTheDocument();
    expect(screen.getByText(/Skill name is required/i)).toBeInTheDocument();
  });
});
