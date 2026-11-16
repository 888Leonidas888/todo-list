export interface TodoItem {
  id: number;
  title: string;
  description: string | null;
  isCompleted: boolean;
  createdAt: string;
}

export interface CreateTodoPayload {
  title: string;
  description: string | null;
}

export interface UpdateTodoPayload {
  title: string;
  description: string | null;
  isCompleted: boolean;
}
