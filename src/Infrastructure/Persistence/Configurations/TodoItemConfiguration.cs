using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo_App.Domain.Entities;

namespace Todo_App.Infrastructure.Persistence.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Note)
            .HasMaxLength(2);
        builder.Property(t => t.BackroundColour)
            .HasMaxLength(50);
        builder
            .HasMany(t => t.Tags)
            .WithMany(t => t.TodoItems)
            .UsingEntity(j => j.ToTable("TodoItemTags"));
    }
}
