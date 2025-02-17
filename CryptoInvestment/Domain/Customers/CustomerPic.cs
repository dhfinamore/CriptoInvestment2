namespace CryptoInvestment.Domain.Customers;

public class CustomerPic
{
    public int IdCustomerPic { get; set; }
    public int IdCustomer { get; set; }
    public string? Type { get; set; } = null!;
    public byte[]? PictureFront { get; set; } = null!;
    public byte[]? PictureBack { get; set; }
    public string? RejectMessage { get; set; }
}
