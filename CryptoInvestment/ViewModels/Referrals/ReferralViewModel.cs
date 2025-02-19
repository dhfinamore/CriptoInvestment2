namespace CryptoInvestment.ViewModels.Referrals;

public class ReferralViewModel
{
    public int CustomerId { get; set; }
    public string ReferralCode { get; set; } = null!;   
    public List<Referral> ReferralList { get; set; } = [];
}

public class Referral
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}