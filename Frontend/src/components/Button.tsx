import { type ComponentChildren, type FunctionalComponent } from "preact";

export const Button: FunctionalComponent<{
  children: ComponentChildren;
  variant?: "primary" | "ghost";
  disabled?: boolean;
  onClick?: () => void;
  type?: "button" | "submit" | "reset";
}> = ({ children, variant = "primary", disabled, onClick, type = "button" }) => (
  <button
    type={type}
    class={variant === "ghost" ? "button button--ghost" : "button"}
    disabled={disabled}
    onClick={onClick}
  >
    {children}
  </button>
);
