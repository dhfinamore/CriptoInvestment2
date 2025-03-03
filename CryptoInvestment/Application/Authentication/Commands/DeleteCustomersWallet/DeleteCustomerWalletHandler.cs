using CryptoInvestment.Application.Common.Interface;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.DeleteCustomersWallet;

public class DeleteCustomerWalletHandler : IRequestHandler<DeleteCustomerWalletCommand, ErrorOr<Success>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerWalletHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteCustomerWalletCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
        
        if (customer is null)
            return Error.NotFound(description: "Customer not found");
        
        var wallet = await _customerRepository.GetCustomerWithdrawalWallets(request.CustomerId);
        
        var walletToDelete = wallet.FirstOrDefault(w => w.Id == request.WalletId);
        
        if (walletToDelete is null)
            return Error.NotFound(description: "Wallet not found");
        
        await _customerRepository.DeleteCustomerWithdrawalWallet(walletToDelete);
        await _unitOfWork.CommitChangesAsync();

        return Result.Success;
    }
}