using CryptoInvestment.Domain.InvPlan;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvPlans.Queries;

public record GetInvPlanByIdQuery(int InvPlanId) : IRequest<ErrorOr<InvPlan>>;