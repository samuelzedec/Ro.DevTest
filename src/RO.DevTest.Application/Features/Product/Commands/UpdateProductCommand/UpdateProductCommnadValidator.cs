using FluentValidation;

namespace RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand;

public class UpdateProductCommnadValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommnadValidator()
    {
        When(p => !string.IsNullOrWhiteSpace(p.Name), () =>
        {
            RuleFor(p => p.Name)
                .MaximumLength(255)
                .WithMessage("Nome do produto deve ter no máximo 255 caracteres");
        });

        When(p => !string.IsNullOrWhiteSpace(p.Description), () =>
        {
            RuleFor(p => p.Description)
                .MaximumLength(455)
                .WithMessage("Descrição do produto deve ter no máximo 455 caracteres");
        });
        
        RuleFor(p => p.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Preço unitário deve ser maior que zero")
            .PrecisionScale(18, 2, false)
            .WithMessage("Preço unitário deve ter no máximo 2 casas decimais");
        
        RuleFor(p => p.AvailableQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantidade disponível deve ser maior ou igual a zero");
    }
}