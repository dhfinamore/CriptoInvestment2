using CryptoInvestment.Domain.InvPlan;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvPlans.Queries.ListInvPlanQuery;

public record ListInvPlansQuery() : IRequest<ErrorOr<List<InvPlan>>>;
