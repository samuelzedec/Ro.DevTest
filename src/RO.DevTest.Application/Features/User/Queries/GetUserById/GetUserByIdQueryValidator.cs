using FluentValidation;

namespace RO.DevTest.Application.Features.User.Queries.GetUserById;

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(u => u.Id)
            .NotNull()
            .NotEmpty()
            .WithMessage("O campo id é obrigatório");
    }
}