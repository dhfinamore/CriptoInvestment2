using CryptoInvestment.Domain.Customers;
using CryptoInvestment.ViewModels.Authentication;

namespace CryptoInvestment.ViewModels.CustomerConfiguration;

public class CustomerConfigurationViewModel
{
    public int CustomerId { get; set; }
    public CustomerInformationViewModel CustomerInformation { get; set; }
    public ResetPasswordViewModel ResetPassword { get; set; }
    public SecurityQuestionsViewModel SetSecurityQuestion { get; set; }
    public CustomerBeneficiaryViewModel CustomerBeneficiary { get; set; }
    public List<CustomerWithdrawalWallet> CustomerWithdrawalWallets { get; set; } = [];
    public CustomerWithdrawalWalletsViewModel CustomerWithdrawalWallet { get; set; }
    public CustomerPic? CustomerPic { get; set; }
    public int DocsValidated { get; set; }
    public WithdrawalsPasswordViewModel WithdrawalsPassword { get; set; }
    public bool IsWithdrawalsPasswordSet { get; set; }
    
    public CustomerConfigurationViewModel()
    {
        ResetPassword = new ResetPasswordViewModel();
        SetSecurityQuestion = new SecurityQuestionsViewModel();
        CustomerBeneficiary = new CustomerBeneficiaryViewModel();
        WithdrawalsPassword = new WithdrawalsPasswordViewModel();
        CustomerInformation = new CustomerInformationViewModel();
        CustomerWithdrawalWallet = new CustomerWithdrawalWalletsViewModel();
    }
}
