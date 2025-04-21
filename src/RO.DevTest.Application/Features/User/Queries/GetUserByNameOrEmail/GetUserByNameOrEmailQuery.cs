using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.User.Queries.GetUserByNameOrEmail;

public record GetUserByNameOrEmailQuery(string NameOrEmail) : IRequest<Result<GetUserByNameOrEmailResponse>>;