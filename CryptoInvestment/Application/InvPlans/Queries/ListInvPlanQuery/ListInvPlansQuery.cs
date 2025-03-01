using CryptoInvestment.Domain.InvPlans;

using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvPlans.Queries.ListInvPlanQuery;

public record ListInvPlansQuery(int? CustomerId = null) : IRequest<ErrorOr<List<InvPlan>>>;
