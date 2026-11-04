using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Todos;

namespace TodoApp.Infrastructure.Persistence;

/// <summary>
/// Mapping kept out of the domain entity, so the Domain layer stays free of EF Core.
/// Written as it would be for a relational database, so switching provider needs no changes here.
/// </summary>
internal sealed class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.HasKey(todo => todo.Id);
        builder.Property(todo => todo.Id).ValueGeneratedNever();

        builder.Property(todo => todo.Title)
            .IsRequired()
            .HasMaxLength(TodoItem.TitleMaxLength);

        builder.Property(todo => todo.CreatedAt).IsRequired();
        builder.Property(todo => todo.CompletedAt);

        builder.Ignore(todo => todo.IsCompleted);
    }
}
