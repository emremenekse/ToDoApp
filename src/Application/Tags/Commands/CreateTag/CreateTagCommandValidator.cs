using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Todo_App.Application.Tags.Commands.CreateTag;
public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(50)
            .NotEmpty()
            .WithMessage("Tag name is required and should not exceed 50 characters.");

        RuleFor(v => v.Color)
            .MaximumLength(20)
            .WithMessage("Color code should not exceed 20 characters.");
    }
}
