using System.ComponentModel.DataAnnotations;
using CryptoInvestment.Domain.InvAssets;
using CryptoInvestment.Domain.InvOperations;
using CryptoInvestment.Domain.InvPlans;

namespace CryptoInvestment.ViewModels.Deposit;

public class DepositViewModel
{
    public int CustomerId { get; set; }
    
    [Required(ErrorMessage = "Debe seleccionar un plan para continuar con su depósito")]
    public int InvPlanId { get; set; }
    
    [Required(ErrorMessage = "Debe seleccionar una moneda para continuar con su depósito")]
    public int SelectedCurrencyId { get; set; }
    
    [Required(ErrorMessage = "El monto a depositar no es válido")]
    [Range(0, int.MaxValue, ErrorMessage = "El monto a depositar no es válido")]
    public decimal DepositAmount { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "El monto a depositar no es válido")]
    public decimal ReinversionAmount { get; set; }
    
    [Range(0, 100, ErrorMessage = "El porcentaje de reinversión no es válido")]
    public decimal ReinvestPercent { get; set; }
    public decimal MonthProfit { get; set; }
    
    public int EndType { get; set; }
    
    public List<InvPlan> InvPlans = [];
    public List<InvCurrency> InvCurrencies = [];
    public List<InvBalance> InvBalances = [];
}