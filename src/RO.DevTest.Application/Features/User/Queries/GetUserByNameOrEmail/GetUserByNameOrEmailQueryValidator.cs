using FluentValidation;

namespace RO.DevTest.Application.Features.User.Queries.GetUserByNameOrEmail;

public class GetUserByNameOrEmailQueryValidator : AbstractValidator<GetUserByNameOrEmailQuery>
{
    public GetUserByNameOrEmailQueryValidator()
    {
        RuleFor(u => u.NameOrEmail)
            .NotNull()
            .NotEmpty()
            .WithMessage("O campo Nome/Email é obrigatório");
    }
}