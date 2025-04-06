
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;

namespace Todo_App.Application.Tags.Queries.GetTags;
public record GetTagsQuery : IRequest<TagsVm>;

public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, TagsVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTagsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TagsVm> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        return new TagsVm
        {
            Tags = await _context.Tags
                .OrderByDescending(t => t.UsageCount)
                .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken)
        };
    }
}
