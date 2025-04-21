using FluentValidation;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductsByAdminIdQuery;

public class GetProductsByAdminIdQueryValidator : AbstractValidator<GetProductsByAdminIdQuery>
{
    public GetProductsByAdminIdQueryValidator()
    {
        RuleFor(p => p.PageSize)
            .GreaterThan(0)
            .WithMessage("O número de produtos retornados deve ser maior que zero")
            .LessThanOrEqualTo(100)
            .WithMessage("O número máximo de produtos retornados por página é 100");

        RuleFor(p => p.PageNumber)
            .GreaterThan(0)
            .WithMessage("O número da página deve ser maior que zero");
    }
}