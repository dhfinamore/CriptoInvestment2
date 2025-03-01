using CryptoInvestment.Domain.InvPlans;

using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvPlans.Queries;

public record GetInvPlanByIdQuery(int InvPlanId) : IRequest<ErrorOr<InvPlan>>;