using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.User.Queries.GetUserByNameOrEmailQuery;

public record GetUserByNameOrEmailQuery(string NameOrEmail) : IRequest<Result<GetUserByNameOrEmailResponse>>;