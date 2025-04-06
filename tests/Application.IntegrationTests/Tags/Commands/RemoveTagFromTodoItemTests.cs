using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Tags.Commands.AddTagToTodoItem;
using Todo_App.Application.Tags.Commands.CreateTag;
using Todo_App.Application.Tags.Commands.RemoveTagFromTodoItem;
using Todo_App.Application.TodoItems.Commands.CreateTodoItem;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.IntegrationTests.Tags.Commands;
using static Testing;

public class RemoveTagFromTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var userId = await RunAsDefaultUserAsync();

        var tagId = await SendAsync(new CreateTagCommand
        {
            Name = "Important",
            Color = "#FF0000"
        });

        var command = new RemoveTagFromTodoItemCommand
        {
            TodoItemId = 99,
            TagId = tagId
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldRemoveTagFromTodoItem()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        var tagId = await SendAsync(new CreateTagCommand
        {
            Name = "Important",
            Color = "#FF0000"
        });

        await SendAsync(new AddTagToTodoItemCommand
        {
            TodoItemId = itemId,
            TagId = tagId
        });

        var command = new RemoveTagFromTodoItemCommand
        {
            TodoItemId = itemId,
            TagId = tagId
        };

        await SendAsync(command);

        var tag = await FindAsync<Tag>(tagId);

        tag.Should().NotBeNull();
        tag!.UsageCount.Should().Be(0);
    }
}
