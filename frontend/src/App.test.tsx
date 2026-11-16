import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import App from './App';

describe('App - Gestión de Tareas', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_API_URL', 'http://localhost:8080');
    vi.restoreAllMocks();
  });

  it('muestra la lista de tareas obtenida del backend y permite crear una tarea', async () => {
    const initialTodos = [
      {
        id: 1,
        title: 'Configurar repositorio',
        description: 'Usar template corporativo',
        isCompleted: false,
        createdAt: '2026-11-16T00:00:00Z',
      },
    ];

    const createdTodo = {
      id: 2,
      title: 'Ejecutar pruebas',
      description: 'Validar xUnit y Vitest',
      isCompleted: false,
      createdAt: '2026-11-16T01:00:00Z',
    };

    const fetchMock = vi
      .spyOn(globalThis, 'fetch')
      .mockResolvedValueOnce(
        new Response(JSON.stringify(initialTodos), {
          status: 200,
          headers: { 'Content-Type': 'application/json' },
        })
      )
      .mockResolvedValueOnce(
        new Response(JSON.stringify(createdTodo), {
          status: 201,
          headers: { 'Content-Type': 'application/json' },
        })
      );

    render(<App />);

    expect(await screen.findByText('Configurar repositorio')).toBeInTheDocument();

    await userEvent.type(screen.getByLabelText('Título'), 'Ejecutar pruebas');
    await userEvent.type(
      screen.getByLabelText('Descripción (opcional)'),
      'Validar xUnit y Vitest'
    );
    await userEvent.click(screen.getByRole('button', { name: 'Agregar tarea' }));

    expect(await screen.findByText('Ejecutar pruebas')).toBeInTheDocument();
    expect(fetchMock).toHaveBeenCalledTimes(2);
  });

  it('muestra un mensaje de error explícito si la API falla (cero fallbacks silenciosos)', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValueOnce(
      new Response(JSON.stringify({ error: 'Error de conexión con la base de datos' }), {
        status: 500,
        headers: { 'Content-Type': 'application/json' },
      })
    );

    render(<App />);

    await waitFor(() => {
      expect(screen.getByRole('alert')).toHaveTextContent(
        'Error de conexión con la base de datos'
      );
    });
  });
});
