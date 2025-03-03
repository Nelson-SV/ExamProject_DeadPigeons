using DataAccess.Models;

namespace Service.TransferModels.Responses;

public class GameIdDto
{
    public string Guid { get; set; } = null!;


    public static GameIdDto FromGame(Game game)
    {
        return new GameIdDto()
        {
            Guid = game.Guid
        };
    }
}