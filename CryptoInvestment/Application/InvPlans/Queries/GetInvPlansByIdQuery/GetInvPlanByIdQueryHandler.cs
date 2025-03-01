using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.InvPlans;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvPlans.Queries.GetInvPlansByIdQuery;

public class GetInvPlanByIdQueryHandler : IRequestHandler<GetInvPlanByIdQuery, ErrorOr<InvPlan>>
{
    private readonly IInvPlanRepository _invPlanRepository;

    public GetInvPlanByIdQueryHandler(IInvPlanRepository invPlanRepository)
    {
        _invPlanRepository = invPlanRepository;
    }

    public async Task<ErrorOr<InvPlan>> Handle(GetInvPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var invPlan = await _invPlanRepository.GetInvPlanByIdAsync(request.InvPlanId);
        
        return invPlan is not null
            ? invPlan
            : Error.NotFound(description: "Investment plan not found.");
    }
}
