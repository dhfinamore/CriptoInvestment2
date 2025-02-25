using System.ComponentModel.DataAnnotations;

using CryptoInvestment.Domain.InvOperations;
using CryptoInvestment.Domain.InvPlans;

namespace CryptoInvestment.ViewModels.Deposit;

public class DepositViewModel
{
    public int CustomerId { get; set; }
    
    [Required(ErrorMessage = "Debe seleccionar un plan para continuar con su depósito")]
    public int InvPlanId { get; set; }
    
    public int SelectedCurrencyId { get; set; }
    public List<InvPlan> InvPlans = [];
    public List<InvCurrency> InvCurrencies = [];
}