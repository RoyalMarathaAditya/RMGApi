import { Autocomplete, Chip, TextField } from '@mui/material';
import { availableTechnologies } from '../mock/projects';

interface TechnologySelectorProps {
  error?: string;
  label?: string;
  onBlur?: () => void;
  onChange: (technologies: string[]) => void;
  value: string[];
}

export default function TechnologySelector({
  error,
  label = 'Technology Stack',
  onBlur,
  onChange,
  value,
}: TechnologySelectorProps) {
  return (
    <Autocomplete
      filterSelectedOptions
      multiple
      onBlur={onBlur}
      onChange={(_, selectedTechnologies) => onChange(selectedTechnologies)}
      options={availableTechnologies}
      renderInput={(params) => (
        <TextField {...params} error={Boolean(error)} helperText={error} label={label} placeholder="Search technology" />
      )}
      renderTags={(selected, getTagProps) =>
        selected.map((option, index) => {
          const { key, ...tagProps } = getTagProps({ index });

          return <Chip key={key} label={option} size="small" {...tagProps} />;
        })
      }
      value={value}
    />
  );
}
