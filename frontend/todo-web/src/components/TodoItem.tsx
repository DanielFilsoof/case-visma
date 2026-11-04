import type { Todo } from '../api/types'

interface TodoItemProps {
  todo: Todo
  onToggle: (todo: Todo) => void
  onDelete: (id: string) => void
}

export function TodoItem({ todo, onToggle, onDelete }: TodoItemProps) {
  return (
    <li className={todo.isCompleted ? 'todo-item completed' : 'todo-item'}>
      <label>
        <input type="checkbox" checked={todo.isCompleted} onChange={() => onToggle(todo)} />
        <span>{todo.title}</span>
      </label>
      <button className="delete" aria-label={`Slet "${todo.title}"`} onClick={() => onDelete(todo.id)}>
        ✕
      </button>
    </li>
  )
}
