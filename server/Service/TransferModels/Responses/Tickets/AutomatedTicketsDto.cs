using DataAccess.Models;

namespace Service.TransferModels.Responses;

public class AutomatedTicketsDto
{
    public Guid Guid { get; set; }

    public DateTime PurchaseDate { get; set; }

    public int[] Sequence { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public int PriceValue { get; set; }

    public bool IsActive { get; set; }
    public static  AutomatedTicketsDto FromEntity(AutomatedTicket ticket)
    {
        return new AutomatedTicketsDto
        {
            Guid = ticket.Guid,
            PurchaseDate = ticket.PurchaseDate,
            Sequence =  ticket.Sequence.ToArray(),
            UserId = ticket.UserId,
            PriceValue = ticket.PriceValue,
            IsActive = ticket.IsActive,
        };
    }
    
    public static  AutomatedTicket ToEntity(AutomatedTicketsDto ticket)
    {
        return new AutomatedTicket
        {
            Guid = ticket.Guid,
            PurchaseDate = ticket.PurchaseDate,
            Sequence =  ticket.Sequence.ToArray(),
            UserId = ticket.UserId,
            PriceValue = ticket.PriceValue,
            IsActive = ticket.IsActive
        };
    }
    
    public static  GameTicket ToGameTicketEntity(AutomatedTicketsDto ticket)
    {
        return new GameTicket
        {
            GUID = ticket.Guid,
            PurchaseDate = ticket.PurchaseDate,
            Sequence =  ticket.Sequence.ToArray(),
            UserId = ticket.UserId,
            PriceValue = ticket.PriceValue,
        };
    }
}