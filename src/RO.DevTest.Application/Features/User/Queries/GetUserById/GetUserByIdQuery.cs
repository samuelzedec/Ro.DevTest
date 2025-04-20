using MediatR;
using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Application.Features.User.Queries.GetUserById;

public record GetUserByIdQuery : IRequest<Result<GetUserByIdResponse>>;