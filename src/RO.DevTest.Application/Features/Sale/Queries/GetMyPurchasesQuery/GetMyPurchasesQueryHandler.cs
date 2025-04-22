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
    ILogger<GetMyPurchasesQueryHandler> logger) 
    : IRequestHandler<GetMyPurchasesQuery, Result<List<GetMyPurchasesResponse>>>
{
    public async Task<Result<List<GetMyPurchasesResponse>>> Handle(GetMyPurchasesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (currentUserService.IsAdmin())
                return Result<List<GetMyPurchasesResponse>>.Failure(messages: "Only customers have purchases");

            var sales = await saleRepository
                .GetQueryable(
                    s => s.CustomerId == Guid.Parse(currentUserService.GetCurrentUserId()) && s.DeletedAt == null,
                    s => s.Product)
                .ToListAsync(cancellationToken);
            
            return Result<List<GetMyPurchasesResponse>>.Success(
                sales.Select(s => new GetMyPurchasesResponse(s)).ToList(),
                messages: "Purchases found");
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Result<List<GetMyPurchasesResponse>>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}