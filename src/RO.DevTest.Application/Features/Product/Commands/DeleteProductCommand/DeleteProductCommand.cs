using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Product.Commands.DeleteProductCommand;

public record DeleteProductCommand(Guid ProductId) : IRequest<Result<DeleteProductResponse>>;