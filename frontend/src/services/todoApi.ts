import type { CreateTodoPayload, TodoItem, UpdateTodoPayload } from '../types/todo';

function getApiBaseUrl(): string {
  const configuredUrl = import.meta.env.VITE_API_URL;
  if (!configuredUrl || configuredUrl.trim() === '') {
    throw new Error('VITE_API_URL no está configurada.');
  }
  return configuredUrl.replace(/\/+$/, '');
}

async function requestJson<T>(path: string, init?: RequestInit): Promise<T> {
  const baseUrl = getApiBaseUrl();
  const response = await fetch(`${baseUrl}${path}`, {
    ...init,
    headers: {
      'Content-Type': 'application/json',
      ...(init?.headers ?? {}),
    },
  });

  if (!response.ok) {
    let errorMessage = `Error HTTP ${response.status}`;
    try {
      const errorBody = (await response.json()) as { error?: string };
      if (errorBody?.error) {
        errorMessage = errorBody.error;
      }
    } catch {
      // Mantiene el código HTTP cuando el cuerpo no es JSON
    }
    throw new Error(errorMessage);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export const todoApi = {
  getAll: (): Promise<TodoItem[]> => requestJson<TodoItem[]>('/api/todos'),

  create: (payload: CreateTodoPayload): Promise<TodoItem> =>
    requestJson<TodoItem>('/api/todos', {
      method: 'POST',
      body: JSON.stringify(payload),
    }),

  update: (id: number, payload: UpdateTodoPayload): Promise<TodoItem> =>
    requestJson<TodoItem>(`/api/todos/${id}`, {
      method: 'PUT',
      body: JSON.stringify(payload),
    }),

  remove: (id: number): Promise<void> =>
    requestJson<void>(`/api/todos/${id}`, {
      method: 'DELETE',
    }),
};
