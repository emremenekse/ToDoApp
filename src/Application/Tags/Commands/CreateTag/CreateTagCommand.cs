using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Tags.Commands.CreateTag;
public record CreateTagCommand : IRequest<int>
{
    public string? Name { get; init; }
    public string? Color { get; init; }
}

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var entity = new Tag
        {
            Name = request.Name,
            Color = request.Color,
            UsageCount = 0
        };

        _context.Tags.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
