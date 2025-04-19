using FluentValidation;

namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(u => u.Id)
            .NotEmpty()
            .NotEmpty()
            .WithMessage("O campo id é obrigatório");
    }
}