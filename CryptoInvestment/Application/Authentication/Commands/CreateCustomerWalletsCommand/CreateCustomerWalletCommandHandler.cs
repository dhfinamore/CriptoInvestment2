using CryptoInvestment.Application.Common.Interface;
using CryptoInvestment.Domain.Customers;
using ErrorOr;
using MediatR;

namespace CryptoInvestment.Application.Authentication.Commands.CreateCustomerWalletsCommand;

public class CreateCustomerWalletCommandHandler : IRequestHandler<CreateCustomerWalletCommand, ErrorOr<Success>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerWalletCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(CreateCustomerWalletCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
        
        if (customer is null)
        {
            return Error.NotFound(description :"Customer not found");
        }
        
        if (request.WalletId.HasValue)
        {
            var wallets = await _customerRepository.GetCustomerWithdrawalWallets(request.CustomerId);
            
            var customerWallet = wallets.FirstOrDefault(x => x.Id == request.WalletId);
            if (customerWallet is null)
            {
                return Error.NotFound(description: "Wallet not found");
            }

            customerWallet.WalletName = request.WalletName;
            customerWallet.InvCurrency = request.InvCurrency;
            customerWallet.WalletAccount = request.Account;
            
            await _customerRepository.UpdateCustomerWithdrawalWalletAsync(customerWallet);
        }
        else
        {
            var wallet = new CustomerWithdrawalWallet()
            {
                CustomerId = request.CustomerId,
                InvCurrency = request.InvCurrency,
                WalletName = request.WalletName,
                WalletAccount = request.Account,
                Used = request.Used
            };
        
            await _customerRepository.AddCustomerWithdrawalWalletAsync(wallet);
        }
        await _unitOfWork.CommitChangesAsync();

        return Result.Success;
    }
}