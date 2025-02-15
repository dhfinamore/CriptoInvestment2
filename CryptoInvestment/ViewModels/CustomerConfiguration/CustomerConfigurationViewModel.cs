using CryptoInvestment.ViewModels.Authentication;

namespace CryptoInvestment.ViewModels.CustomerConfiguration;

public class CustomerConfigurationViewModel
{
    public int CustomerId { get; set; }
    public ResetPasswordViewModel ResetPassword { get; set; }
    public SecurityQuestionsViewModel SetSecurityQuestion { get; set; }
    
    public CustomerConfigurationViewModel()
    {
        ResetPassword = new ResetPasswordViewModel();
        SetSecurityQuestion = new SecurityQuestionsViewModel();
    }
}