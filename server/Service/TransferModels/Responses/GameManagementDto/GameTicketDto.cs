using DataAccess.Models;

namespace Service.TransferModels.Responses.GameManagementDto;

public class GameTicketDto
{
  //  public Guid Guid { get; set; }

    public DateTime PurchaseDate { get; set; }

    public int[] Sequence { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public int PriceValue { get; set; }

  //  public string GameId { get; set; } = null!;


    public static GameTicketDto FromEntity(GameTicket gameTicket)
    {
        return new GameTicketDto
        {
           // Guid = gameTicket.Guid,
            PurchaseDate = gameTicket.PurchaseDate,
            Sequence = gameTicket.Sequence,
            UserId = gameTicket.UserId,
            PriceValue = gameTicket.PriceValue,
       //     GameId = gameTicket.GameId,
        };
    }
}