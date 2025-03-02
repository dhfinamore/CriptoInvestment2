using System.ComponentModel.DataAnnotations;

namespace CryptoInvestment.ViewModels.CustomerConfiguration;

public class CustomerInformationViewModel
{
    [Required(ErrorMessage = "El Nombre es obligatorio.")]
    public string Name { get; set; } = null!;
    
    [Required(ErrorMessage = "El Apellido Paterno es obligatorio.")]
    public string ApellidoPaterno { get; set; } = null!;
    public string? ApellidoMaterno { get; set; }
    
    [Required(ErrorMessage = "El campo de número de teléfono es obligatorio.")]
    public string? Phone { get; set; }
    public string? Coutry { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? BirthDate { get; set; }
}