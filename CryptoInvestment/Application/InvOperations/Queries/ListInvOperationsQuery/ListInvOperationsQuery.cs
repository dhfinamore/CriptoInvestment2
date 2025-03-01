using CryptoInvestment.Domain.InvOperations;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvOperations.Queries.ListInvOperationsQuery;

public record ListInvOperationsQuery (int CustomerId) : IRequest<ErrorOr<List<InvOperation>>>;