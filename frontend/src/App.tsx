import { useEffect, useState, type FormEvent } from 'react';
import { todoApi } from './services/todoApi';
import type { TodoItem } from './types/todo';
import './App.css';

export function App() {
  const [todos, setTodos] = useState<TodoItem[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const [title, setTitle] = useState<string>('');
  const [description, setDescription] = useState<string>('');
  const [editingId, setEditingId] = useState<number | null>(null);
  const [editingCompleted, setEditingCompleted] = useState<boolean>(false);

  const loadTodos = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await todoApi.getAll();
      setTodos(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cargar las tareas.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadTodos();
  }, []);

  const resetForm = () => {
    setTitle('');
    setDescription('');
    setEditingId(null);
    setEditingCompleted(false);
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);

    try {
      if (editingId !== null) {
        const updated = await todoApi.update(editingId, {
          title,
          description: description.trim() ? description : null,
          isCompleted: editingCompleted,
        });
        setTodos((prev) => prev.map((item) => (item.id === updated.id ? updated : item)));
      } else {
        const created = await todoApi.create({
          title,
          description: description.trim() ? description : null,
        });
        setTodos((prev) => [created, ...prev]);
      }
      resetForm();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al guardar la tarea.');
    }
  };

  const handleToggleComplete = async (todo: TodoItem) => {
    setError(null);
    try {
      const updated = await todoApi.update(todo.id, {
        title: todo.title,
        description: todo.description,
        isCompleted: !todo.isCompleted,
      });
      setTodos((prev) => prev.map((item) => (item.id === updated.id ? updated : item)));
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al actualizar el estado de la tarea.');
    }
  };

  const handleStartEdit = (todo: TodoItem) => {
    setEditingId(todo.id);
    setTitle(todo.title);
    setDescription(todo.description ?? '');
    setEditingCompleted(todo.isCompleted);
  };

  const handleDelete = async (id: number) => {
    setError(null);
    try {
      await todoApi.remove(id);
      setTodos((prev) => prev.filter((item) => item.id !== id));
      if (editingId === id) {
        resetForm();
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al eliminar la tarea.');
    }
  };

  return (
    <main className="app-container">
      <header className="app-header">
        <h1>Gestión de Tareas</h1>
      </header>

      {error && (
        <div role="alert" className="error-banner">
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit} className="todo-form">
        <div className="form-field">
          <label htmlFor="todo-title">Título</label>
          <input
            id="todo-title"
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Ej: Revisar despliegue en QA"
            required
          />
        </div>

        <div className="form-field">
          <label htmlFor="todo-description">Descripción (opcional)</label>
          <input
            id="todo-description"
            type="text"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Detalles adicionales"
          />
        </div>

        <div className="form-actions">
          <button type="submit">
            {editingId !== null ? 'Guardar cambios' : 'Agregar tarea'}
          </button>
          {editingId !== null && (
            <button type="button" onClick={resetForm} className="btn-secondary">
              Cancelar
            </button>
          )}
        </div>
      </form>

      {loading ? (
        <p>Cargando tareas...</p>
      ) : todos.length === 0 ? (
        <p>No hay tareas registradas.</p>
      ) : (
        <ul className="todo-list">
          {todos.map((todo) => (
            <li key={todo.id} className={todo.isCompleted ? 'todo-item completed' : 'todo-item'}>
              <div className="todo-content">
                <input
                  type="checkbox"
                  checked={todo.isCompleted}
                  onChange={() => void handleToggleComplete(todo)}
                  aria-label={`Completar ${todo.title}`}
                />
                <div>
                  <strong>{todo.title}</strong>
                  {todo.description && <p>{todo.description}</p>}
                </div>
              </div>
              <div className="todo-actions">
                <button type="button" onClick={() => handleStartEdit(todo)}>
                  Editar
                </button>
                <button
                  type="button"
                  onClick={() => void handleDelete(todo.id)}
                  className="btn-danger"
                >
                  Eliminar
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </main>
  );
}

export default App;
