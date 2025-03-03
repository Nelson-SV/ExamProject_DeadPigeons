
namespace Service.TransferModels.Responses.BalanceDto;

public class UploadedCloudImageResponse
{
    public string Name { get; set; } = null!;
    public string Bucket { get; set; } = null!;
    public DateTime TimeCreated { get; set; }
    public DateTime Updated { get; set; }
    public string MediaLink { get; set; } = null!;
}