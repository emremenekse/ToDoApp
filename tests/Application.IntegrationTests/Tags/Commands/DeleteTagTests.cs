using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.Tags.Commands.CreateTag;
using Todo_App.Application.Tags.Commands.DeleteTag;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.IntegrationTests.Tags.Commands;
using static Testing;

public class DeleteTagTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTagId()
    {
        var command = new DeleteTagCommand(99);

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteTag()
    {
        var userId = await RunAsDefaultUserAsync();

        var tagId = await SendAsync(new CreateTagCommand
        {
            Name = "Important",
            Color = "#FF0000"
        });

        await SendAsync(new DeleteTagCommand(tagId));

        var tag = await FindAsync<Tag>(tagId);

        tag.Should().BeNull();
    }
}
