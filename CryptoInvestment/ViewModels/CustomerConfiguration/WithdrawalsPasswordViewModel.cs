using System.ComponentModel.DataAnnotations;

namespace CryptoInvestment.ViewModels.CustomerConfiguration;

public class WithdrawalsPasswordViewModel
{
    [Required(ErrorMessage = "La contraseña para retiros es inválida")]
    public string Character1 { get; set; } = null!;
    [Required(ErrorMessage = "La contraseña para retiros es inválida")]
    public string Character2 { get; set; } = null!;
    [Required(ErrorMessage = "La contraseña para retiros es inválida")]
    public string Character3 { get; set; } = null!;
    [Required(ErrorMessage = "La contraseña para retiros es inválida")]
    public string Character4 { get; set; } = null!;
    [Required(ErrorMessage = "La contraseña para retiros es inválida")]
    public string Character5 { get; set; } = null!;
    [Required(ErrorMessage = "La contraseña para retiros es inválida")]
    public string Character6 { get; set; } = null!;
}