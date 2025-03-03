using DataAccess.Models;

namespace Service.TransferModels.Requests;

public class CreateTicketDto
{
    public DateTime PurchaseDate { get; set; } 

    public string UserId { get; set; } = null!;

    public string GameId { get; set; } = null!;
    
    public int[] Sequence { get; set; } = null!;
    public int PriceValue { get; set; }

    public bool IsAutomated { get; set; } = false;


    public static List<GameTicket> ToTickets( Dictionary<Guid, CreateTicketDto> allTicketsMap)
    {
        return allTicketsMap.Select(pair => new GameTicket
        {
            GUID = pair.Key,
            PurchaseDate = pair.Value.PurchaseDate,
            Sequence = pair.Value.Sequence,
            UserId = pair.Value.UserId,
            GameId = pair.Value.GameId,
            PriceValue = pair.Value.PriceValue,
        }).ToList();
    }



   
    public static List<AutomatedTicket> ToAutomatedTickets(Dictionary<Guid, CreateTicketDto> automatedTicketMap)
    {
        return automatedTicketMap.Select(pair => new AutomatedTicket
        {
            Guid = pair.Key,
            PurchaseDate = pair.Value.PurchaseDate,
            Sequence = pair.Value.Sequence,
            UserId = pair.Value.UserId,
            PriceValue = pair.Value.PriceValue,
            IsActive = true,
        }).ToList();
    }
}