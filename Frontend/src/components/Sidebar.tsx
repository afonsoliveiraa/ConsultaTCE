import { type FunctionalComponent } from "preact";
import { Icon, type IconName } from "./Icon";

const menuGroups: { title: string; items: { label: string; active?: boolean; icon: IconName }[] }[] = [
  {
    title: "Principal",
    items: [
      { label: "Menu", icon: "menu" },
      { label: "Início", active: true, icon: "home" },
      { label: "Cadastros", icon: "file" },
      { label: "Unidades Funcionais", icon: "bank" },
      { label: "Documentos", icon: "file" },
      { label: "Consultas", icon: "search" },
      { label: "Relatórios", icon: "chart" },
      { label: "Usuários", icon: "user" },
    ],
  },
];

export const Sidebar: FunctionalComponent<{ open: boolean; onToggle: () => void }> = ({ open, onToggle }) => {
  return (
    <aside class={`app-sidebar${open ? "" : " app-sidebar--collapsed"}`}>
      <div class="app-sidebar__top">
        <div class="app-sidebar__toggle">
          <button class="sidebar-toggle-button" type="button" onClick={onToggle} aria-label="Alternar menu">
            <Icon name="menu" className="sidebar-svg-icon" />
          </button>
        </div>

        {open ? (
          <label class="app-sidebar__search">
            <span>Buscar no menu...</span>
            <input value="" />
          </label>
        ) : null}
      </div>

      <nav class="app-sidebar__nav" aria-label="Menu principal">
        {menuGroups.map((group) => (
          <section class="sidebar-group">
            {open ? <small>{group.title}</small> : null}
            {group.items.map((item) => (
              <button
                class={`sidebar-link${item.active ? " is-active" : ""}${open ? "" : " is-icon-only"}`}
                type="button"
                title={item.label}
              >
                <span class="sidebar-link__icon">
                  <Icon name={item.icon} className="sidebar-svg-icon" />
                </span>
                {open ? <span class="sidebar-link__label">{item.label}</span> : null}
              </button>
            ))}
          </section>
        ))}
      </nav>
    </aside>
  );
};
