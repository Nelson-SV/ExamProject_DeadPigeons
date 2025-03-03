using DataAccess.Models;

namespace Service.TransferModels.Responses.Tickets;

public class GameTicketDetailedDto
{
    public Guid Guid { get; set; }
    public string TicketNumber => Guid.ToString().Split('-')[0].ToUpper(); 
    public DateTime PurchaseDate { get; set; }
    public string FormattedPurchaseDate => PurchaseDate.ToString("yyyy/MM/dd");
    public int[] Sequence { get; set; }
    public string UserId { get; set; }
    public int PriceValue { get; set; }
    public int[]? ExtractedNumbers { get; set; }
    public decimal? Winnings { get; set; }
    public string GameId { get; set; }

    public static GameTicketDetailedDto FromEntity(GameTicket ticket)
    {
        return new GameTicketDetailedDto
        {
            Guid = ticket.GUID,
            PurchaseDate = ticket.PurchaseDate,
            Sequence = ticket.Sequence,
            UserId = ticket.UserId,
            PriceValue = ticket.PriceValue,
            GameId = ticket.GameId,
        };
    }
}