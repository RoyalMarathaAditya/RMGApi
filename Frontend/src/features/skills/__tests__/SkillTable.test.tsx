import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import SkillTable from '../components/SkillTable';
import type { Skill } from '../types';

const skills: Skill[] = [
  {
    category: 'Frontend',
    description: 'React development',
    employeeCount: 12,
    id: 1,
    skillCode: 'SKL-001',
    skillName: 'React',
    status: 'Active',
  },
];

describe('SkillTable', () => {
  it('renders rows and calls action handlers', async () => {
    const onView = jest.fn();
    const onEdit = jest.fn();
    const onDelete = jest.fn();

    render(<SkillTable onDelete={onDelete} onEdit={onEdit} onView={onView} skills={skills} />);

    expect(await screen.findByText('React')).toBeInTheDocument();
    fireEvent.click(screen.getByLabelText(/View React/i));
    fireEvent.click(screen.getByLabelText(/Edit React/i));
    fireEvent.click(screen.getByLabelText(/Delete React/i));

    await waitFor(() => {
      expect(onView).toHaveBeenCalledWith(skills[0]);
      expect(onEdit).toHaveBeenCalledWith(skills[0]);
      expect(onDelete).toHaveBeenCalledWith(skills[0]);
    });
  });
});
