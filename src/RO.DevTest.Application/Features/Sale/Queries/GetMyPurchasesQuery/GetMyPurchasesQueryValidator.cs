using FluentValidation;

namespace RO.DevTest.Application.Features.Sale.Queries.GetMyPurchasesQuery;

public class GetMyPurchasesQueryValidator : AbstractValidator<GetMyPurchasesQuery>
{
    public GetMyPurchasesQueryValidator()
    {
        RuleFor(s => s.PageNumber)
            .GreaterThan(0)
            .WithMessage("O número da página deve ser maior que zero");

        RuleFor(s => s.PageSize)
            .GreaterThan(0)
            .WithMessage("O número de vendas retornadas tem que ser maior que zero")
            .LessThanOrEqualTo(100)
            .WithMessage("O número máximo de vendas retornados por página é 100");
    }
}