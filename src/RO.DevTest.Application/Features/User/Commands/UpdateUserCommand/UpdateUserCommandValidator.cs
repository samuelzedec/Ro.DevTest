using FluentValidation;

namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        When(u => !string.IsNullOrEmpty(u.UserName), () => {
            RuleFor(u => u.UserName)
                .MaximumLength(255)
                .WithMessage("O nome de usuário deve ter no máximo 255 caracteres");
        });
        
        When(u => !string.IsNullOrEmpty(u.Name), () => {
            RuleFor(u => u.Name)
                .MaximumLength(255)
                .WithMessage("O nome deve ter no máximo 255 caracteres");
        });
        
        When(u => !string.IsNullOrEmpty(u.Email), () => {
            RuleFor(u => u.Email)
                .EmailAddress()
                .WithMessage("Formato de e-mail inválido")
                .MaximumLength(255)
                .WithMessage("O e-mail deve ter no máximo 255 caracteres");
        });
    }
}