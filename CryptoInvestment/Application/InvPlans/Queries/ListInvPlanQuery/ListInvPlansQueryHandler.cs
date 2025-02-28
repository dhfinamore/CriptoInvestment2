using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.InvPlans;
using CryptoInvestment.Infrastucture.InvPlans.Persistance;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvPlans.Queries.ListInvPlanQuery;

public class ListInvPlansQueryHandler : IRequestHandler<ListInvPlansQuery, ErrorOr<List<InvPlan>>>
{
    private readonly IInvPlanRepository _invPlanRepository;

    public ListInvPlansQueryHandler(IInvPlanRepository invPlanRepository)
    {
        _invPlanRepository = invPlanRepository;
    }

    public async Task<ErrorOr<List<InvPlan>>> Handle(ListInvPlansQuery request, CancellationToken cancellationToken)
    {
        var invPlans = await _invPlanRepository.GetInvPlansAsync(request.CustomerId);
        
        return invPlans.Count > 0
            ? invPlans
            : Error.NotFound(description: "No investment plans found.");
    }
}
