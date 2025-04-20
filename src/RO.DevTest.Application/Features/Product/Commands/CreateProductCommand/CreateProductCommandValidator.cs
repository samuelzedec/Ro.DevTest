using FluentValidation;

namespace RO.DevTest.Application.Features.Product.Commands.CreateProductCommand;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Nome do produto é obrigatório")
            .MaximumLength(255)
            .WithMessage("Nome do produto deve ter no máximo 255 caracteres");
            
        RuleFor(p => p.Description)
            .NotEmpty()
            .WithMessage("Descrição do produto é obrigatória")
            .MaximumLength(455)
            .WithMessage("Descrição do produto deve ter no máximo 455 caracteres");
            
        RuleFor(p => p.UnitPrice)
            .NotEmpty()
            .WithMessage("Preço unitário é obrigatório")
            .GreaterThan(0)
            .WithMessage("Preço unitário deve ser maior que zero")
            .PrecisionScale(18, 2, false)
            .WithMessage("Preço unitário deve ter no máximo 2 casas decimais");
            
        RuleFor(p => p.AvailableQuantity)
            .NotEmpty()
            .WithMessage("Quantidade disponível é obrigatória")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantidade disponível deve ser maior ou igual a zero");
            
        RuleFor(p => p.ProductCategory)
            .NotNull()
            .WithMessage("Categoria do produto é obrigatória")
            .IsInEnum()
            .WithMessage("Categoria do produto inválida");
    }
}