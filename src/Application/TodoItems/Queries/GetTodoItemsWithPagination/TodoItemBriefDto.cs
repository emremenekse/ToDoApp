﻿using Todo_App.Application.Common.Mappings;
using Todo_App.Application.Tags.Queries.GetTags;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public class TodoItemBriefDto : IMapFrom<TodoItem>
{
    public int Id { get; set; }

    public int ListId { get; set; }

    public string? Title { get; set; }
    public string? BackroundColour { get; set; }
    public IList<TagDto>? Tags { get; set; }


    public bool Done { get; set; }
}
