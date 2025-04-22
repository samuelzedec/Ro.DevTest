using FluentValidation;

namespace RO.DevTest.Application.Features.Sale.Queries.GetSaleByIdQuery;

public class GetSaleByIdQueryValidator : AbstractValidator<GetSaleByIdQuery>
{
    public GetSaleByIdQueryValidator()
    {
        RuleFor(s => s.SaleId)
            .NotNull()
            .WithMessage("O Id da venda é obrigatório");
    }
}