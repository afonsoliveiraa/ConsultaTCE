import { type FunctionalComponent } from "preact";

export const SelectField: FunctionalComponent<{
  label: string;
  value: string;
  options: { label: string; value: string }[];
  onChange: (value: string) => void;
}> = ({ label, value, options, onChange }) => (
  <label class="field">
    <span>{label}</span>
    <select
      value={value}
      onChange={(event) => onChange((event.currentTarget as HTMLSelectElement).value)}
    >
      {options.map((option) => (
        <option key={option.value} value={option.value}>
          {option.label}
        </option>
      ))}
    </select>
  </label>
);
