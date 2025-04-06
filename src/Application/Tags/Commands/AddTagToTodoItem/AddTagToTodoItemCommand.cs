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

namespace Todo_App.Application.Tags.Commands.AddTagToTodoItem;
public record AddTagToTodoItemCommand : IRequest
{
    public int TodoItemId { get; init; }
    public int TagId { get; init; }
}

public class AddTagToTodoItemCommandHandler : IRequestHandler<AddTagToTodoItemCommand>
{
    private readonly IApplicationDbContext _context;

    public AddTagToTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(AddTagToTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoItem = await _context.TodoItems
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == request.TodoItemId, cancellationToken);

        if (todoItem == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.TodoItemId);
        }

        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.Id == request.TagId, cancellationToken);

        if (tag == null)
        {
            throw new NotFoundException(nameof(Tag), request.TagId);
        }

        if (!todoItem.Tags.Any(t => t.Id == tag.Id))
        {
            todoItem.Tags.Add(tag);
            tag.UsageCount++;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
