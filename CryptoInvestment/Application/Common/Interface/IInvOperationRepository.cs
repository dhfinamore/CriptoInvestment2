using CryptoInvestment.Domain.InvOperations;

namespace CryptoInvestment.Application.Common.Interface;

public interface IInvOperationRepository
{
    Task CreateInvOperationAsync(InvOperation invOperation);
    Task<List<InvOperation>> GetInvOperationsAsync(int customerId);
    Task<InvOperation?> GetInvOperationByIdAsync(int id);
    Task<List<InvAction>> GetInvActionsAsync();
    Task<List<InvCurrency>> GetInvCurrenciesAsync();
}
