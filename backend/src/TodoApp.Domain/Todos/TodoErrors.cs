using TodoApp.Domain.Common;

namespace TodoApp.Domain.Todos;

public static class TodoErrors
{
    public static readonly Error TitleRequired =
        Error.Validation("Todo.TitleRequired", "Titel er påkrævet.");

    public static readonly Error TitleTooLong =
        Error.Validation("Todo.TitleTooLong", $"Titlen må højst være {TodoItem.TitleMaxLength} tegn.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("Todo.NotFound", $"Opgaven med id '{id}' blev ikke fundet.");
}
