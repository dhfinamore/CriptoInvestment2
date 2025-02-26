using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.InvOperations;
using MediatR;
using ErrorOr;

namespace CryptoInvestment.Application.InvOperations.Queries.ListInvCurrenciesQuery;

public class ListInvCurrenciesQueryHandler : IRequestHandler<ListInvCurrenciesQuery, ErrorOr<List<InvCurrency>>>
{
    private readonly IInvOperationRepository _invOperationRepository;

    public ListInvCurrenciesQueryHandler(IInvOperationRepository invOperationRepository)
    {
        _invOperationRepository = invOperationRepository;
    }

    public async Task<ErrorOr<List<InvCurrency>>> Handle(ListInvCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var currencies = await _invOperationRepository.GetInvCurrenciesAsync();
        return currencies.Count > 0 ? currencies : [];
    }
}