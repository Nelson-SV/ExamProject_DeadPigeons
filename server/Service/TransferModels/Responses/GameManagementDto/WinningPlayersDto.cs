using Common.SharedModels;

namespace Service.TransferModels.Responses.GameManagementDto;

public class WinningPlayersDto
{
    public List<WinningPlayer>? WinningPlayers { get; set; }
    public Pagination? Pagination { get; set; }
}