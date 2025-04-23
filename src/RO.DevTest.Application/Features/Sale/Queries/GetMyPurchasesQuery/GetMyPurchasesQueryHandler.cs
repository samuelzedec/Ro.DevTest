using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RO.DevTest.Application.Contracts.Infrastructure.Services;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.Sale.Queries.GetMyPurchasesQuery;

public class GetMyPurchasesQueryHandler(
    ISaleRepository saleRepository,
    ICurrentUserService currentUserService,
    IValidator<GetMyPurchasesQuery> validator,
    ILogger<GetMyPurchasesQueryHandler> logger) 
    : IRequestHandler<GetMyPurchasesQuery, Result<List<GetMyPurchasesResponse>>>
{
    public async Task<Result<List<GetMyPurchasesResponse>>> Handle(GetMyPurchasesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<List<GetMyPurchasesResponse>>.Failure(messages:
                    validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }
            if (currentUserService.IsAdmin())
                return Result<List<GetMyPurchasesResponse>>.Failure(messages: "Only customers have purchases");

            var sales = await saleRepository
                .GetQueryable(
                    s => s.CustomerId == Guid.Parse(currentUserService.GetCurrentUserId()) && s.DeletedAt == null,
                    s => s.Product)
                .Skip((request.PageNumber -1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new GetMyPurchasesResponse(s))
                .ToListAsync(cancellationToken);
            
            return Result<List<GetMyPurchasesResponse>>.Success(sales, messages: "Purchases found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<List<GetMyPurchasesResponse>>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}