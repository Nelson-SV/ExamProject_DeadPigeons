namespace Service.TransferModels.Responses.GameManagementDto;

public class WinningPlayer
{
    public string PlayerId { get; set; } = null!;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public int WinningCount { get; set; }
    public List<Guid> WinningTicketsIds { get; set; } = [];
}