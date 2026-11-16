# Todo List - Gestión de Tareas (Golden Path - Vicoders)

Aplicación Fullstack para gestión de tareas (`todo-list`) construida sobre el template corporativo gobernado por TI.

---

## 🏗️ Arquitectura y Estructura del Proyecto

### Backend (`backend/TodoList.sln` - `.NET 10` Minimal APIs en Clean Architecture)
- **`TodoList.Domain`:** Entidad `TodoItem` (`Id`, `Title`, `Description`, `IsCompleted`, `CreatedAt`) e interfaz `ITodoRepository`. Cero dependencias externas.
- **`TodoList.Application`:** DTOs (`TodoDto`, `CreateTodoDto`, `UpdateTodoDto`) y caso de uso centralizado `TodoService` (DRY, validaciones de dominio).
- **`TodoList.Infrastructure`:** `AppDbContext` (Entity Framework Core SQL Server / InMemory para pruebas), `TodoRepository` e inyección de dependencias (`DependencyInjection.cs`) sin fallbacks silenciosos.
- **`TodoList.Api`:** Endpoints Minimal API (`/health` y `/api/todos`) y configuración CORS.
- **`TodoList.Tests`:** Pruebas de arquitectura (`ArchitectureTests.cs`) y pruebas de integración CRUD (`TodoApiTests.cs`) con `xUnit` y `WebApplicationFactory`.

### Frontend (`frontend/` - React 19 + TypeScript + Vite en Node.js 26 LTS)
- Cliente HTTP centralizado (`src/services/todoApi.ts`) consumiendo `import.meta.env.VITE_API_URL` sin fallbacks silenciosos.
- Interfaz CRUD completa (`src/App.tsx`): listar, crear, editar título/descripción, alternar estado completado y eliminar tareas.
- Pruebas automatizadas en `src/App.test.tsx` con `Vitest` y `@testing-library/react`.

---

## 🔌 Endpoints de la API

| Método | Ruta | Descripción |
| :--- | :--- | :--- |
| `GET` | `/health` | Estado de salud del servicio |
| `GET` | `/api/todos` | Lista todas las tareas ordenadas por fecha de creación |
| `GET` | `/api/todos/{id}` | Obtiene una tarea por su `Id` |
| `POST` | `/api/todos` | Crea una nueva tarea (`Title` obligatorio, `Description` opcional) |
| `PUT` | `/api/todos/{id}` | Actualiza `Title`, `Description` e `IsCompleted` de una tarea |
| `DELETE` | `/api/todos/{id}` | Elimina una tarea existente |

---

## 📋 Gobernanza del Repositorio

| Archivo / Ruta | Propósito |
| :--- | :--- |
| [`MEMORY.md`](./MEMORY.md) | **Fuente de Verdad del proyecto.** Registra los planes aprobados por el usuario, decisiones de arquitectura y el estado secuencial de las tareas. |
| [`AGENTS.md`](./AGENTS.md) | Reglas globales leídas automáticamente por **Codex**, **Antigravity** y **GitHub Copilot**. |
| [`.agents/rules/golden-path-rules.md`](./.agents/rules/golden-path-rules.md) | Reglas inquebrantables (`always_on`): Conventional Commits v1.0.0, ramas obligatorias, PR siempre con `gh`, DRY, cero fallbacks silenciosos y cero datos sensibles. |
| [`.agents/skills/scaffold-clean-fullstack/SKILL.md`](./.agents/skills/scaffold-clean-fullstack/SKILL.md) | Skill para inicializar la solución `.NET 10` en 4 capas de Clean Architecture + `React TS` (`Node 26 LTS`) + `Docker`. |
| [`.agents/skills/add-crud-feature/SKILL.md`](./.agents/skills/add-crud-feature/SKILL.md) | Skill para implementar módulos de negocio capa por capa con pruebas obligatorias. |
| [`.agents/skills/git-pr-workflow/SKILL.md`](./.agents/skills/git-pr-workflow/SKILL.md) | Skill para gestionar ramas, auditoría de secretos/archivos basura, Conventional Commits y creación de Pull Requests con `gh`. |
| [`plan.md`](./plan.md) | Definición operativa de las 4 fases del **Golden Path** (Local, Filtro TI, QA en GCP y Pase a Producción). |
| [`.github/copilot-instructions.md`](./.github/copilot-instructions.md) | Criterios de auditoría automática para **GitHub Copilot Code Review** en cada Pull Request. |
| [`.github/workflows/ci-cd.yml`](./.github/workflows/ci-cd.yml) | Pipeline automatizado: Filtro de TI (Tests + Clean Architecture + Cero Secretos), Despliegue a Cloud Run (QA) y Pase a Producción con aprobación de 1 clic. |

---

## 🛡️ Las 11 Reglas de Oro del Flujo de Trabajo

1. **Conventional Commits v1.0.0:** Todo commit sigue `<tipo>(<alcance>): <descripción>` (`feat`, `fix`, `docs`, `refactor`, `test`, `chore`, etc.).
2. **Prohibido trabajar sobre `main`:** Todo cambio se implementa en una rama (`feat/*`, `fix/*`, `docs/*`, `chore/*`).
3. **No Auto-Merge y Secuencialidad:** No se fusiona a `main` sin orden explícita del usuario y no se avanza a una tarea nueva sin terminar la anterior.
4. **Uso obligatorio de `gh` CLI:** Todo cambio se sube al remoto y se gestiona mediante `gh`.
5. **Pull Request (`PR`) siempre:** Cada entrega genera un PR hacia `main` (`gh pr create`).
6. **Cero archivos basura:** No se suben archivos ajenos al core; ante cualquier duda, el agente pregunta al usuario.
7. **Cero exposición de datos sensibles (3 Entornos Preconfigurados):** Ninguna credencial o secreto en el repositorio:
   * **Local:** Variable de entorno de sistema preconfigurada hacia `localhost\SQLEXPRESS` (`VicoderLocalDb` con autenticación integrada de Windows).
   * **QA (Cloud Run):** Secreto `db-connection-string-qa` en **GCP Secret Manager** apuntando a `BooksDb_QA`.
   * **Producción (Cloud Run):** Secreto `db-connection-string-prod` en **GCP Secret Manager** apuntando a `BooksDb_Prod` (requiere aprobación de 1 clic del revisor `888Leonidas888`).
8. **Código DRY (Don't Repeat Yourself):** Reutilización obligatoria de lógica y componentes.
9. **Cero fallbacks silenciosos:** Prohibido ocultar errores o asumir comportamientos; ante cualquier duda técnica se consulta al usuario.
10. **Sincronización de `README.md` y `MEMORY.md`:** Ambos documentos se mantienen actualizados en cada entrega.
11. **`MEMORY.md` como Fuente de Verdad:** Una vez aprobado el plan por el usuario, se registra en `MEMORY.md` antes de ejecutar.
