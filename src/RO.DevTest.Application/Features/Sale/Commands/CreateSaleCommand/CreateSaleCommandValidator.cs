using FluentValidation;

namespace RO.DevTest.Application.Features.Sale.Commands.CreateSaleCommand;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(s => s.ProductId)
            .NotEmpty()
            .WithMessage("O id do produto é obrigatório");

        RuleFor(s => s.PaymentMethod)
            .NotNull()
            .WithMessage("O tipo do pagamento é obrigatório")
            .IsInEnum()
            .WithMessage("Tipo de pagamento inválido");

        RuleFor(s => s.Quantity)
            .NotNull()
            .WithMessage("Quantidade é obrigatória")
            .GreaterThan(0)
            .WithMessage("A quantidade deve ser maior que 0");
    }
}