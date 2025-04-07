using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.Tags.Commands.CreateTag;
using Todo_App.Domain.Entities;
using Todo_App.Application.Common.Exceptions;

namespace Todo_App.Application.IntegrationTests.Tags.Commands;
using static Testing;

public class CreateTagTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateTagCommand();

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateTag()
    {
        var userId = await RunAsDefaultUserAsync();

        var command = new CreateTagCommand
        {
            Name = "Important",
            Color = "#FF0000"
        };

        var tagId = await SendAsync(command);

        var tag = await FindAsync<Tag>(tagId);

        tag.Should().NotBeNull();
        tag!.Name.Should().Be(command.Name);
        tag.Color.Should().Be(command.Color);
        tag.UsageCount.Should().Be(0);
        tag.CreatedBy.Should().Be(userId);
        tag.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        tag.LastModifiedBy.Should().Be(userId);
        tag.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
