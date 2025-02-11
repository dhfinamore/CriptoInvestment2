namespace CryptoInvestment.ViewModels.Authentication;

public class RegisterViewModel
{
    public int? ParentId { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string FirstFamilyName { get; set; } = null!;
    public string SecondFamilyName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public bool TermsAndConditions { get; set; }
    public bool AcceptPromotions { get; set; }
}