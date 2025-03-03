using Common.SharedModels;
using Service.TransferModels.Responses;

namespace Service.TransferModels.Requests.Admin;

public class GameRequestDto
{
    public GameIdDto? GameId { get; set; }
    public Pagination? Pagination { get; set; }
}