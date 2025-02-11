namespace CryptoInvestment.Domain.Customers;

public class Customer
{
    public int IdCustomer { get; set; }
    public int? IdParent { get; set; }
    public bool Active { get; set; }
    public string? Nombre { get; set; }
    public string? ApellidoPaterno { get; set; }
    public string? ApellidoMaterno { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? Phone { get; set; }
    public string Email { get; set; }
    public string? PasswdLogin { get; set; }
    public bool? EmailValidated { get; set; }
    public int? ClPais { get; set; }
    public int? IdEstado { get; set; }
    public string? City { get; set; }
    public string? PasswdWithdrawal { get; set; }
    public DateTime? LockedUp { get; set; }
    public int? FailedLoginAttempts { get; set; } = 0;
    public bool? AcceptPromoEmail { get; set; }
    public DateTime? Arrival { get; set; }
    public bool? DocsValidated { get; set; }
    
    public Customer()
    {
        Active = true;
        EmailValidated = false;
        AcceptPromoEmail = false;
    }
}
