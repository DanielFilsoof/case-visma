import { useCallback, useEffect, useState } from 'react'
import { todoApi, type TodoApi } from '../api/todoApi'
import type { Todo } from '../api/types'

const toMessage = (error: unknown) => (error instanceof Error ? error.message : 'Noget gik galt.')

export function useTodos(api: TodoApi = todoApi) {
  const [todos, setTodos] = useState<Todo[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let isCancelled = false

    api
      .getAll()
      .then((loaded) => !isCancelled && setTodos(loaded))
      .catch((e: unknown) => !isCancelled && setError(toMessage(e)))
      .finally(() => !isCancelled && setIsLoading(false))

    return () => {
      isCancelled = true
    }
  }, [api])

  const run = useCallback(async (action: () => Promise<void>): Promise<boolean> => {
    setError(null)
    try {
      await action()
      return true
    } catch (e) {
      setError(toMessage(e))
      return false
    }
  }, [])

  const replace = (updated: Todo) =>
    setTodos((current) => current.map((todo) => (todo.id === updated.id ? updated : todo)))

  const addTodo = (title: string) =>
    run(async () => {
      const created = await api.create({ title })
      setTodos((current) => [...current, created])
    })

  const toggleTodo = (todo: Todo) =>
    run(async () => replace(todo.isCompleted ? await api.reopen(todo.id) : await api.complete(todo.id)))

  const deleteTodo = (id: string) =>
    run(async () => {
      await api.remove(id)
      setTodos((current) => current.filter((todo) => todo.id !== id))
    })

  return { todos, isLoading, error, addTodo, toggleTodo, deleteTodo }
}
