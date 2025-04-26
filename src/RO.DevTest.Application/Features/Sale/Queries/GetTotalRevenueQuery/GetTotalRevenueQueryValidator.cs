using FluentValidation;

namespace RO.DevTest.Application.Features.Sale.Queries.GetTotalRevenueQuery;

public class GetTotalRevenueQueryValidator : AbstractValidator<GetTotalRevenueQuery>
{
    public GetTotalRevenueQueryValidator()
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
}