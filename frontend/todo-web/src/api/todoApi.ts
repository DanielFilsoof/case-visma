import type { CreateTodoRequest, ProblemDetails, Todo } from './types'

const BASE_URL = '/api/todos'

export class ApiError extends Error {
  readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

export interface TodoApi {
  getAll(): Promise<Todo[]>
  create(request: CreateTodoRequest): Promise<Todo>
  complete(id: string): Promise<Todo>
  reopen(id: string): Promise<Todo>
  remove(id: string): Promise<void>
}

async function request<T>(url: string, init?: RequestInit): Promise<T> {
  const response = await fetch(url, {
    ...init,
    headers: { 'Content-Type': 'application/json', ...init?.headers },
  })

  if (!response.ok) {
    throw await toApiError(response)
  }

  return response.status === 204 ? (undefined as T) : ((await response.json()) as T)
}

async function toApiError(response: Response): Promise<ApiError> {
  const fallback = `Forespørgslen fejlede med status ${response.status}`

  try {
    const problem = (await response.json()) as ProblemDetails
    return new ApiError(response.status, problem.detail ?? problem.title ?? fallback)
  } catch {
    return new ApiError(response.status, fallback)
  }
}

export const todoApi: TodoApi = {
  getAll: () => request<Todo[]>(BASE_URL),
  create: (body) => request<Todo>(BASE_URL, { method: 'POST', body: JSON.stringify(body) }),
  complete: (id) => request<Todo>(`${BASE_URL}/${id}/complete`, { method: 'PATCH' }),
  reopen: (id) => request<Todo>(`${BASE_URL}/${id}/reopen`, { method: 'PATCH' }),
  remove: (id) => request<void>(`${BASE_URL}/${id}`, { method: 'DELETE' }),
}
