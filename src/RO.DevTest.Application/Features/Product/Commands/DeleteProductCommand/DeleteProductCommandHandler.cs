using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Product.Commands.DeleteProductCommand;

public class DeleteProductCommandHandler(
    IProductRepository productRepository,
    ICurrentUserService currentUserService,
    IValidator<DeleteProductCommand> validator,
    ILogger<DeleteProductCommandHandler> logger)
    : IRequestHandler<DeleteProductCommand, Result<DeleteProductResponse>>
{
    public async Task<Result<DeleteProductResponse>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<DeleteProductResponse>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var product = await productRepository.GetAsync(
                cancellationToken,
                p => p.Id == request.ProductId 
                     && p.DeletedAt == null
                     && p.AdminId == Guid.Parse(currentUserService.GetCurrentUserId()));

            if (product is null)
                return Result<DeleteProductResponse>.Failure(StatusCodes.Status404NotFound, messages: "Produto não encontrado");
            
            product.DeletedAt = DateTime.UtcNow;
            await productRepository.UpdateAsync(product, cancellationToken);
            return Result<DeleteProductResponse>.Success(new DeleteProductResponse(product), messages: "Produto deletado com sucesso");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<DeleteProductResponse>.Failure(StatusCodes.Status500InternalServerError, 
                "Ocorreu um erro inesperado, consulte o arquivo de hoje na pasta Logs");
        }
    }
}
