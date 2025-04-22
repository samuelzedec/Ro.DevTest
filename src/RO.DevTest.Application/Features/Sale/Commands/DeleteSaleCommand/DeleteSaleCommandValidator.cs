using FluentValidation;

namespace RO.DevTest.Application.Features.Sale.Commands.DeleteSaleCommand;

public class DeleteSaleCommandValidator : AbstractValidator<DeleteSaleCommand>
{
    public DeleteSaleCommandValidator()
    {
        RuleFor(s => s.SaleId)
            .NotEmpty()
            .WithMessage("Id da compra é obrigatório");
    }
}