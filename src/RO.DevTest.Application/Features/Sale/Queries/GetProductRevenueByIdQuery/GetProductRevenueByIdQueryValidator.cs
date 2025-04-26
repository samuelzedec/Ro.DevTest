using FluentValidation;

namespace RO.DevTest.Application.Features.Sale.Queries.GetProductRevenueByIdQuery;

public class GetProductRevenueByIdQueryValidator : AbstractValidator<GetProductRevenueByIdQuery>
{
    public GetProductRevenueByIdQueryValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("O Id do produto é obrigatório")
            .Must(BeValidGuid)
            .WithMessage("O Id do produto deve ser um GUID válido");
        
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