using FluentValidation;

namespace RO.DevTest.Application.Features.Sale.Commands.UpdateSaleCommand;

public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {
        RuleFor(s => s.SaleId)
            .NotEmpty()
            .WithMessage("Id da compra é obrigatório");
        
        RuleFor(s => s.EPaymentMethod)
            .NotNull()
            .WithMessage("O método de pagamento é obrigastório")
            .IsInEnum()
            .WithMessage("Método de pagamento inválido");
    }
}