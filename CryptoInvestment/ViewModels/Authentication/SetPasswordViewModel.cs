using System.ComponentModel.DataAnnotations;

namespace CryptoInvestment.ViewModels.Authentication;

public class SetPasswordViewModel
{
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "La contraseña es requerida")]
    [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
        ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial")]
    public string Password { get; set; } = null!;
    
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = null!;
}