using FluentValidation;

namespace RO.DevTest.Application.Features.Product.Commands.DeleteProductCommand;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(p => p.ProductId)
            .NotEmpty()
            .WithMessage("O Id do produto é obrigatório");
    }
}