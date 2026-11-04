import { useState, type FormEvent } from 'react'

const TITLE_MAX_LENGTH = 200

interface TodoFormProps {
  /** Resolves to true when the todo was created. */
  onAdd: (title: string) => Promise<boolean>
}

export function TodoForm({ onAdd }: TodoFormProps) {
  const [title, setTitle] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault()
    if (!title.trim()) return

    setIsSubmitting(true)
    const created = await onAdd(title)
    setIsSubmitting(false)
    if (created) setTitle('')
  }

  return (
    <form className="todo-form" onSubmit={handleSubmit}>
      <input
        aria-label="Ny opgave"
        placeholder="Hvad skal gøres?"
        value={title}
        maxLength={TITLE_MAX_LENGTH}
        onChange={(event) => setTitle(event.target.value)}
        disabled={isSubmitting}
        autoFocus
      />
      <button type="submit" disabled={isSubmitting || !title.trim()}>
        Tilføj
      </button>
    </form>
  )
}
