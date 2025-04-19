using FluentValidation;

namespace RO.DevTest.Application.Features.Auth.Commands.RefreshTokenCommand;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(rt => rt.UserId)
            .NotNull()
            .NotEmpty()
            .WithMessage("Id é obrigatório");

        RuleFor(rt => rt.RefreshToken)
            .NotNull()
            .NotEmpty()
            .WithMessage("Refresh Token é obrigatório");
    }
}