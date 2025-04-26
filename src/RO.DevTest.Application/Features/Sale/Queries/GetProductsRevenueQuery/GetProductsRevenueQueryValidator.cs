using FluentValidation;

namespace RO.DevTest.Application.Features.Sale.Queries.GetProductsRevenueQuery;

public class GetProductsRevenueQueryValidator : AbstractValidator<GetProductsRevenueQuery>
{
    public GetProductsRevenueQueryValidator()
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
    }
    
    private bool BeValidGuid(Guid id)
        =>  id != Guid.Empty;
}