import { TodoForm } from './components/TodoForm'
import { TodoList } from './components/TodoList'
import { useTodos } from './hooks/useTodos'

export default function App() {
  const { todos, isLoading, error, addTodo, toggleTodo, deleteTodo } = useTodos()

  const remaining = todos.filter((todo) => !todo.isCompleted).length

  return (
    <main className="app">
      <h1>Opgaver</h1>

      <TodoForm onAdd={addTodo} />

      {error && (
        <p className="error" role="alert">
          {error}
        </p>
      )}

      {isLoading ? (
        <p className="empty">Indlæser…</p>
      ) : (
        <>
          <TodoList todos={todos} onToggle={toggleTodo} onDelete={deleteTodo} />
          <footer className="app-footer">{remaining} tilbage</footer>
        </>
      )}
    </main>
  )
}
