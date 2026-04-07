import { type FunctionalComponent } from "preact";

export const TextField: FunctionalComponent<{
  label: string;
  value: string;
  placeholder?: string;
  onInput: (value: string) => void;
}> = ({ label, value, placeholder, onInput }) => (
  <label class="field">
    <span>{label}</span>
    <input
      value={value}
      placeholder={placeholder}
      onInput={(event) => onInput((event.currentTarget as HTMLInputElement).value)}
    />
  </label>
);
