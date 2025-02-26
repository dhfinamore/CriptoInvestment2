using CryptoInvestment.Domain.InvAssets;

namespace CryptoInvestment.Application.Common.Interface;

public interface IInvAssetsRepository
{
    public Task<List<InvBalance>> GetInvBalances(int customerId);
}