import skillReducer, { addSkill, deleteSkill, setSelectedSkill, setSkills, updateSkill } from '../skillSlice';
import type { Skill } from '../types';

const baseSkill: Skill = {
  category: 'Frontend',
  description: 'React development',
  employeeCount: 4,
  id: 101,
  skillCode: 'SKL-101',
  skillName: 'React',
  status: 'Active',
};

describe('skillSlice', () => {
  it('sets skills', () => {
    const state = skillReducer(undefined, setSkills([baseSkill]));
    expect(state.skills).toHaveLength(1);
    expect(state.skills[0].skillName).toBe('React');
  });

  it('adds a skill', () => {
    const state = skillReducer(
      { error: null, filters: {}, loading: false, selectedSkill: null, skills: [baseSkill] },
      addSkill({
        category: 'Backend',
        description: 'API design',
        skillCode: 'SKL-102',
        skillName: 'REST API Design',
        status: 'Active',
      }),
    );

    expect(state.skills[0].skillName).toBe('REST API Design');
    expect(state.skills[0].employeeCount).toBe(0);
  });

  it('updates, selects, and deletes a skill', () => {
    const updated = { ...baseSkill, skillName: 'React 19' };
    const withUpdate = skillReducer({ error: null, filters: {}, loading: false, selectedSkill: null, skills: [baseSkill] }, updateSkill(updated));
    const selected = skillReducer(withUpdate, setSelectedSkill(updated));
    const deleted = skillReducer(selected, deleteSkill(baseSkill.id));

    expect(withUpdate.skills[0].skillName).toBe('React 19');
    expect(selected.selectedSkill?.id).toBe(baseSkill.id);
    expect(deleted.skills).toHaveLength(0);
    expect(deleted.selectedSkill).toBeNull();
  });
});
