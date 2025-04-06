using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Todo_App.Application.Tags.Commands.RemoveTagFromTodoItem;
public class RemoveTagFromTodoItemCommandValidator : AbstractValidator<RemoveTagFromTodoItemCommand>
{
    public RemoveTagFromTodoItemCommandValidator()
    {
        RuleFor(v => v.TodoItemId)
            .GreaterThan(0)
            .WithMessage("TodoItem ID must be greater than 0.");

        RuleFor(v => v.TagId)
            .GreaterThan(0)
            .WithMessage("Tag ID must be greater than 0.");
    }
}
