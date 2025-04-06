using Microsoft.AspNetCore.Mvc;
using Todo_App.Application.Common.Models;
using Todo_App.Application.Tags.Commands.AddTagToTodoItem;
using Todo_App.Application.Tags.Commands.CreateTag;
using Todo_App.Application.Tags.Commands.DeleteTag;
using Todo_App.Application.Tags.Commands.RemoveTagFromTodoItem;
using Todo_App.Application.Tags.Queries.GetTags;
using Todo_App.Application.Tags.Queries.GetTodoItemsByTag;
using Todo_App.Application.TodoItems.Queries.GetTodoItemsWithPagination;

namespace Todo_App.WebUI.Controllers;

public class TagsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TagsVm>> GetTags()
    {
        return await Mediator.Send(new GetTagsQuery());
    }

    [HttpGet("{tagId}/todoitems")]
    public async Task<ActionResult<PaginatedList<TodoItemBriefDto>>> GetTodoItemsByTag(int tagId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        return await Mediator.Send(new GetTodoItemsByTagQuery { TagId = tagId, PageNumber = pageNumber, PageSize = pageSize });
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTagCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTagCommand(id));

        return NoContent();
    }

    [HttpPost("todoitem/{todoItemId}/tag/{tagId}")]
    public async Task<ActionResult> AddTagToTodoItem(int todoItemId, int tagId)
    {
        await Mediator.Send(new AddTagToTodoItemCommand { TodoItemId = todoItemId, TagId = tagId });

        return NoContent();
    }

    [HttpDelete("todoitem/{todoItemId}/tag/{tagId}")]
    public async Task<ActionResult> RemoveTagFromTodoItem(int todoItemId, int tagId)
    {
        await Mediator.Send(new RemoveTagFromTodoItemCommand { TodoItemId = todoItemId, TagId = tagId });

        return NoContent();
    }
}
