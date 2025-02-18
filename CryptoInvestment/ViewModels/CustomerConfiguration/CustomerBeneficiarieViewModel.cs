using System.ComponentModel.DataAnnotations;
using CryptoInvestment.Domain.Customers;

namespace CryptoInvestment.ViewModels.CustomerConfiguration;

public class CustomerBeneficiaryViewModel
{
    public int? Id { get; set; }
    [Required(ErrorMessage = "Nombre de benefeciario es requerido")]
    public string Name { get; set; } = null!;
    
    [Required(ErrorMessage = "Apellido Paterno de benefeciario es requerido")]
    public string ApePaternal { get; set; } = null!;
    
    public string? ApeMaternal { get; set; }
    public string PhoneNumber { get; set; } = null!;
    
    [Required(ErrorMessage = "Relación con el benefeciario es requerido")]
    public int RelationshipId { get; set; }

    public decimal? Percentage { get; set; }
    
    public List<CustomerBeneficiary> CustomerBeneficiaries { get; set; } = [];
    public List<CustomerRelationship> CustomerRelationships { get; set; } = [];
}