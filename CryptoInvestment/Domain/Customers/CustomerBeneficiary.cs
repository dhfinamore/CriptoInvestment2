namespace CryptoInvestment.Domain.Customers;

public class CustomerBeneficiary
{
    public int IdCustomerBeneficiary { get; set; }
    public int IdCustomer { get; set; }
    public string Nombre { get; set; } = null!;
    public string ApePat { get; set; } = null!;
    public string? ApeMat { get; set; }
    public string Tel { get; set; } = null!;
    public string Relationship { get; set; } = null!;
    public decimal? Porcent { get; set; }
}