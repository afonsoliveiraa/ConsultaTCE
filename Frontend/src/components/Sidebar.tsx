import { type FunctionalComponent } from "preact";
import { useMemo, useState } from "preact/hooks";
import { Icon, type IconName } from "./Icon";

interface SidebarChildItem {
  label: string;
  href: string;
  icon: IconName;
}

interface SidebarItem {
  label: string;
  href?: string;
  icon: IconName;
  children?: SidebarChildItem[];
}

const acquisitionContractsLabel = "Processos";

const menuGroups: { title: string; items: SidebarItem[] }[] = [
  {
    title: "Principal",
    items: [
      { label: "Inicio", href: "/", icon: "home" },
      { label: "Upload de historico", href: "/upload-de-historico", icon: "upload" },
      {
        label: acquisitionContractsLabel,
        icon: "file",
        children: [
          { label: "Consulta do contrato", href: "/consulta-de-contrato", icon: "search" },
        ],
      },
      { label: "Unidades Funcionais", href: "#", icon: "bank" },
      { label: "Documentos", href: "#", icon: "file" },
      { label: "Consultas", href: "#", icon: "search" },
      { label: "Relatorios", href: "#", icon: "chart" },
      { label: "Usuarios", href: "#", icon: "user" },
    ],
  },
];

export const Sidebar: FunctionalComponent<{ open: boolean; onToggle: () => void }> = ({ open, onToggle }) => {
  const currentPath = window.location.pathname;

  // Mantem o submenu de aquisicoes aberto na rota filha ou apos interacao do usuario.
  const defaultExpanded = currentPath === "/consulta-de-contrato";
  const [openGroups, setOpenGroups] = useState<Record<string, boolean>>({
    [acquisitionContractsLabel]: defaultExpanded,
  });

  const groups = useMemo(() => menuGroups, []);

  const toggleGroup = (label: string) => {
    setOpenGroups((current) => ({
      ...current,
      [label]: !current[label],
    }));
  };

  return (
    <aside class={`app-sidebar${open ? "" : " app-sidebar--collapsed"}`}>
      <div class="app-sidebar__top">
        <div class="app-sidebar__toggle">
          <button class="sidebar-toggle-button" type="button" onClick={onToggle} aria-label="Alternar menu">
            <span class="sidebar-toggle-button__icon">
              <Icon name="menu" className="sidebar-svg-icon" />
            </span>
            {open ? <span class="sidebar-toggle-button__label">Diminuir menu</span> : null}
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
        {groups.map((group) => (
          <section key={group.title} class="sidebar-group">
            {open ? <small>{group.title}</small> : null}

            {group.items.map((item) => {
              const hasChildren = Boolean(item.children?.length);
              const groupOpen = hasChildren ? openGroups[item.label] ?? false : false;
              const childActive = item.children?.some((child) => child.href === currentPath) ?? false;
              const itemActive = item.href ? currentPath === item.href : childActive;

              if (hasChildren) {
                return (
                  <div key={item.label} class="sidebar-branch">
                    <button
                      class={`sidebar-link sidebar-link--branch${itemActive ? " is-active" : ""}${open ? "" : " is-icon-only"}`}
                      type="button"
                      title={item.label}
                      onClick={() => toggleGroup(item.label)}
                    >
                      <span class="sidebar-link__icon">
                        <Icon name={item.icon} className="sidebar-svg-icon" />
                      </span>

                      {open ? (
                        <>
                          <span class="sidebar-link__label">{item.label}</span>
                          <span class={`sidebar-link__caret${groupOpen ? " is-open" : ""}`} aria-hidden="true">
                            <svg viewBox="0 0 24 24">
                              <path d="M7 10l5 5 5-5z" fill="currentColor" />
                            </svg>
                          </span>
                        </>
                      ) : null}
                    </button>

                    {open && groupOpen ? (
                      <div class="sidebar-submenu">
                        {item.children?.map((child) => (
                          <a
                            key={child.label}
                            class={`sidebar-link sidebar-link--child${currentPath === child.href ? " is-active" : ""}`}
                            href={child.href}
                            title={child.label}
                          >
                            <span class="sidebar-link__icon">
                              <Icon name={child.icon} className="sidebar-svg-icon" />
                            </span>
                            <span class="sidebar-link__label">{child.label}</span>
                          </a>
                        ))}
                      </div>
                    ) : null}
                  </div>
                );
              }

              return (
                <a
                  key={item.label}
                  class={`sidebar-link${itemActive ? " is-active" : ""}${open ? "" : " is-icon-only"}`}
                  href={item.href}
                  title={item.label}
                >
                  <span class="sidebar-link__icon">
                    <Icon name={item.icon} className="sidebar-svg-icon" />
                  </span>
                  {open ? <span class="sidebar-link__label">{item.label}</span> : null}
                </a>
              );
            })}
          </section>
        ))}
      </nav>
    </aside>
  );
};
