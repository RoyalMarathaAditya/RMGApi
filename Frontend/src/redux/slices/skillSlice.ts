// Redux slice for skill state (uses mock data, no async thunks)
import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import { mockSkills } from '../../features/skills/mock/mockSkills';
import type { Skill, SkillFormValues } from '../../features/skills/types';

interface SkillFilters {
  category?: string;
  search?: string;
  status?: string;
}

export interface SkillState {
  skills: Skill[];
  selectedSkill: Skill | null;
  loading: boolean;
  error: string | null;
  filters: SkillFilters;
}

const initialState: SkillState = {
  error: null,
  filters: {},
  loading: false,
  selectedSkill: null,
  skills: mockSkills,
};

const skillSlice = createSlice({
  initialState,
  name: 'skills',
  reducers: {
    addSkill: (state, action: PayloadAction<SkillFormValues>) => {
      const nextId = Math.max(0, ...state.skills.map((skill) => skill.id)) + 1;
      state.skills.unshift({ id: nextId, employeeCount: 0, ...action.payload });
    },
    deleteSkill: (state, action: PayloadAction<number>) => {
      state.skills = state.skills.filter((skill) => skill.id !== action.payload);
      if (state.selectedSkill?.id === action.payload) {
        state.selectedSkill = null;
      }
    },
    setSelectedSkill: (state, action: PayloadAction<Skill | null>) => {
      state.selectedSkill = action.payload;
    },
    setSkills: (state, action: PayloadAction<Skill[]>) => {
      state.skills = action.payload;
    },
    updateSkill: (state, action: PayloadAction<Skill>) => {
      const index = state.skills.findIndex((skill) => skill.id === action.payload.id);
      if (index >= 0) {
        state.skills[index] = action.payload;
      }
    },
  },
});

export const { addSkill, deleteSkill, setSelectedSkill, setSkills, updateSkill } = skillSlice.actions;
export default skillSlice.reducer;
