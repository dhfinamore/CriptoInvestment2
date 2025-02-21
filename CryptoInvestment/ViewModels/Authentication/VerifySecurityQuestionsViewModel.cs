using System.ComponentModel.DataAnnotations;

namespace CryptoInvestment.ViewModels.Authentication;

public class VerifySecurityQuestionsViewModel
{
    public string Email { get; set; } = null!;
    public string FirstQuestion { get; set; } = null!;
    public string SecondQuestion { get; set; } = null!;
    public string ThirdQuestion { get; set; } = null!;
    
    [Required(ErrorMessage = "La respuesta de esta pregunta es obligatoria")]
    public string? FirstAnswer { get; set; }
    
    [Required(ErrorMessage = "La respuesta de esta pregunta es obligatoria")]
    public string? SecondAnswer { get; set; }
    
    [Required(ErrorMessage = "La respuesta de esta pregunta es obligatoria")]
    public string? ThirdAnswer { get; set; }
}