using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.InvOperations;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.InvOperations.Queries.ListInvActionsQuery;

public class ListInvActionsQueryHandler : IRequestHandler<ListInvActionsQuery, ErrorOr<List<InvAction>>>
{
    private readonly IInvOperationRepository _invOperationRepository;

    public ListInvActionsQueryHandler(IInvOperationRepository invOperationRepository)
    {
        _invOperationRepository = invOperationRepository;
    }

    public async Task<ErrorOr<List<InvAction>>> Handle(ListInvActionsQuery request, CancellationToken cancellationToken)
    {
        var invActions = await _invOperationRepository.GetInvActionsAsync();
        
        return invActions.Count > 0 
            ? invActions 
            : Error.NotFound(description: "No investment actions found");
    }
}
