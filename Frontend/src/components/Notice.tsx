import { type ComponentChildren, type FunctionalComponent } from "preact";

export const Notice: FunctionalComponent<{
  tone?: "error" | "info" | "success";
  children: ComponentChildren;
}> = ({ tone = "info", children }) => (
  <div class={`notice notice--${tone}`}>{children}</div>
);
