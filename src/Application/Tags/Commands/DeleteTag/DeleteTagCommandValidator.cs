using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Todo_App.Application.Tags.Commands.DeleteTag;
public class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator()
    {
        RuleFor(v => v.Id)
            .GreaterThan(0)
            .WithMessage("Tag ID must be greater than 0.");
    }
}
