namespace CryptoInvestment.ViewModels.CustomerConfiguration;

public class UpdateImageViewModel
{
    public bool IsCamera { get; set; }
    public int CustomerId { get; set; }
    public bool RequiresTwoPhotos { get; set; }
    public string Type { get; set; } = null!;
    public string PictureFrontBase64 { get; set; } = null!;
    public string? PictureBackBase64 { get; set; }
}
