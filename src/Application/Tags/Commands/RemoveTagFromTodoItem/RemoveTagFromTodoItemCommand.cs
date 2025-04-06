using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Tags.Commands.RemoveTagFromTodoItem;
public record RemoveTagFromTodoItemCommand : IRequest
{
    public int TodoItemId { get; init; }
    public int TagId { get; init; }
}

public class RemoveTagFromTodoItemCommandHandler : IRequestHandler<RemoveTagFromTodoItemCommand>
{
    private readonly IApplicationDbContext _context;

    public RemoveTagFromTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(RemoveTagFromTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoItem = await _context.TodoItems
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == request.TodoItemId, cancellationToken);

        if (todoItem == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.TodoItemId);
        }

        var tag = todoItem.Tags.FirstOrDefault(t => t.Id == request.TagId);

        if (tag != null)
        {
            todoItem.Tags.Remove(tag);
            tag.UsageCount--;
            if (tag.UsageCount < 0) tag.UsageCount = 0;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
