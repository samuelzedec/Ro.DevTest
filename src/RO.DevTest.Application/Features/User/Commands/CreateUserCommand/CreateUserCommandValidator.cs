using FluentValidation;

namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(u => u.Name)
            .NotEmpty()
            .WithMessage("O nome é obrigatório")
            .MaximumLength(255)
            .WithMessage("O nome deve ter no máximo 255 caracteres");

        RuleFor(u => u.UserName)
            .NotEmpty()
            .WithMessage("O nome de usuário é obrigatório")
            .MaximumLength(255)
            .WithMessage("O nome de usuário deve ter no máximo 255 caracteres")
            .Matches("^[a-z0-9]*$")
            .WithMessage("O nome de usuário deve conter apenas letras minúsculas e números, sem espaços");

        RuleFor(u => u.Email)
            .NotEmpty()
            .WithMessage("O campo e-mail é obrigatório")
            .EmailAddress()
            .WithMessage("Formato de e-mail inválido")
            .MaximumLength(255)
            .WithMessage("O e-mail deve ter no máximo 255 caracteres");

        RuleFor(u => u.Password)
            .NotEmpty()
            .WithMessage("A senha é obrigatória")
            .MinimumLength(8)
            .WithMessage("A senha deve ter pelo menos 8 caracteres")
            .Matches("[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula")
            .Matches("[0-9]").WithMessage("A senha deve conter pelo menos um número")
            .Matches("[^a-zA-Z0-9]").WithMessage("A senha deve conter pelo menos um caractere especial");

        RuleFor(u => u.PasswordConfirmation)
            .NotEmpty()
            .WithMessage("A confirmação de senha é obrigatória")
            .Equal(u => u.Password)
            .WithMessage("As senhas não conferem");
    }
}