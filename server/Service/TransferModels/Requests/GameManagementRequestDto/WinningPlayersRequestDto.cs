using Common.SharedModels;
using Service.TransferModels.Responses;

namespace Service.TransferModels.Requests.GameManagementRequestDto;

public class WinningPlayersRequestDto
{
    public GameIdDto? GameId { get; set; }
    public Pagination? Pagination { get; set; }
    public List<int>? WinningSequence { get; set; }
}