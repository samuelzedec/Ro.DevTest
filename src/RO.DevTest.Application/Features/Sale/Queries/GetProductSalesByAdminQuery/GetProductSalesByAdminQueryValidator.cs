using FluentValidation;
using RO.DevTest.Domain.Extensions;

namespace RO.DevTest.Application.Features.Sale.Queries.GetProductSalesByAdminQuery;

public class GetProductSalesByAdminQueryValidator : AbstractValidator<GetProductSalesByAdminQuery>
{
    public GetProductSalesByAdminQueryValidator()
    {
        When(s => s.StartDate.HasValue, () =>
        {
            RuleFor(s => s.EndDate)
                .NotNull()
                .WithMessage("Data final deve ser informada");
        });
        
        When(s => s.EndDate.HasValue && s.StartDate.HasValue, () =>
        {
            RuleFor(s => s.EndDate)
                .GreaterThan(s => s.StartDate)
                .WithMessage("Data final deve ser maior que data de início");
        });
        
        RuleFor(s => s.PageSize)
            .GreaterThan(0)
            .WithMessage("O número de produtos retornados deve ser maior que zero")
            .LessThanOrEqualTo(100)
            .WithMessage("O número máximo de produtos retornados por página é 100");

        RuleFor(s => s.PageNumber)
            .GreaterThan(0)
            .WithMessage("O número da página deve ser maior que zero");
    }
}