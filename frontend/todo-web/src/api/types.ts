export interface Todo {
  id: string
  title: string
  isCompleted: boolean
  createdAt: string
  completedAt: string | null
}

export interface CreateTodoRequest {
  title: string
}

export interface ProblemDetails {
  title?: string
  detail?: string
  status?: number
}
