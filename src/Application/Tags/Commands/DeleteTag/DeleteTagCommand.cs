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

namespace Todo_App.Application.Tags.Commands.DeleteTag;
public record DeleteTagCommand(int Id) : IRequest;

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tags
            .Include(t => t.TodoItems)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Tag), request.Id);
        }

        foreach (var todoItem in entity.TodoItems.ToList())
        {
            todoItem.Tags.Remove(entity);
        }

        _context.Tags.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
