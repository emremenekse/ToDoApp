using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.Common.Mappings;
using Todo_App.Application.Common.Models;
using Todo_App.Application.TodoItems.Queries.GetTodoItemsWithPagination;

namespace Todo_App.Application.Tags.Queries.GetTodoItemsByTag;
public record GetTodoItemsByTagQuery : IRequest<PaginatedList<TodoItemBriefDto>>
{
    public int TagId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoItemsByTagQueryHandler : IRequestHandler<GetTodoItemsByTagQuery, PaginatedList<TodoItemBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoItemsByTagQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TodoItemBriefDto>> Handle(GetTodoItemsByTagQuery request, CancellationToken cancellationToken)
    {
        return await _context.TodoItems
            .Include(t => t.Tags)
            .Where(t => t.Tags.Any(tag => tag.Id == request.TagId))
            .OrderBy(x => x.Title)
            .ProjectTo<TodoItemBriefDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
