namespace CryptoInvestment.Application.Common.Interface;

public interface IUnitOfWork
{
    Task CommitChangesAsync();
}