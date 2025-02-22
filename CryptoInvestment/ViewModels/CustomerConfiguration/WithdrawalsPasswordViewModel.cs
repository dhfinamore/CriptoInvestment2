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
    
    [Compare("Character1", ErrorMessage = "Las contraseñas no coinciden")]
    [Required(ErrorMessage = "La contraseña para retiros es inválida")]
    public string ConfirmCharacter1 { get; set; } = null!;
    
    [Compare("Character2", ErrorMessage = "Las contraseñas no coinciden")]
    [Required(ErrorMessage = "La contraseña para retiros es inválida")]
    public string ConfirmCharacter2 { get; set; } = null!;
    
    [Compare("Character3", ErrorMessage = "Las contraseñas no coinciden")]
    [Required(ErrorMessage = "La contraseña para retiros es inválida")]
    public string ConfirmCharacter3 { get; set; } = null!;
    
    [Compare("Character4", ErrorMessage = "Las contraseñas no coinciden")]
    [Required(ErrorMessage = "La contraseña para retiros es inválida")]
    public string ConfirmCharacter4 { get; set; } = null!;
    
    [Compare("Character5", ErrorMessage = "Las contraseñas no coinciden")]
    [Required(ErrorMessage = "La contraseña para retiros es inválida")]
    public string ConfirmCharacter5 { get; set; } = null!;
    
    [Compare("Character6", ErrorMessage = "Las contraseñas no coinciden")]
    [Required(ErrorMessage = "La contraseña para retiros es inválida")]
    public string ConfirmCharacter6 { get; set; } = null!;
}