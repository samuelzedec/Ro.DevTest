using FluentValidation;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductQuery;

public class GetProductQueryValidator : AbstractValidator<GetProductQuery>
{
    public GetProductQueryValidator()
    {
        RuleFor(p => p.ProductId)
            .NotEmpty()
            .WithMessage("O Id do produto não pode ser vazio");
    }
}