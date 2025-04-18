using FluentValidation;

namespace RO.DevTest.Application.Features.Auth.Commands.LoginCommand;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(l => l.UsernameOrEmail)
            .NotNull()
            .NotEmpty()
            .WithMessage("Esse campo não pode ser vazio");
        
        RuleFor(l => l.Password)
            .MinimumLength(8)
            .WithMessage("O campo senha precisa ter, pelo menos, 8 caracteres");
    }
}