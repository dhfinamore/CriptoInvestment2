using System.ComponentModel.DataAnnotations;

namespace CryptoInvestment.ViewModels.Authentication;

public class RegisterViewModel
{
    public int? ParentId { get; set; }
    
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Ingresa un correo válido.")]
    [MaxLength(70)]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength (45)]
    public string Name { get; set; } = null!;
    
    [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
    [MaxLength(45)]
    public string FirstFamilyName { get; set; } = null!;

    [MaxLength(45)] 
    public string? SecondFamilyName { get; set; }
    
    public string Phone { get; set; } = null!;
    public bool TermsAndConditions { get; set; }
    public bool AcceptPromotions { get; set; }
}