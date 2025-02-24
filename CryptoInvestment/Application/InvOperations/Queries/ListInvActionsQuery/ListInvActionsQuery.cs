using CryptoInvestment.Domain.InvOperations;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvOperations.Queries.ListInvActionsQuery;

public record ListInvActionsQuery() : IRequest<ErrorOr<List<InvAction>>>;
