using DataAccess.Models;

namespace Service.TransferModels.Responses.GameManagementDto;

public class TicketsResponseDto
{
    public Guid Guid { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string FormattedPurchaseDate => PurchaseDate.ToString("yyyy/MM/dd");
    public int PriceValue { get; set; }
    public List<int> PlayedNumbers { get; set; }

    public static TicketsResponseDto FromEntity(GameTicket ticket)
    {
        return new TicketsResponseDto
        {
            Guid = ticket.GUID,
            PurchaseDate = ticket.PurchaseDate,
            PlayedNumbers = ticket.Sequence.ToList(),
            PriceValue = ticket.PriceValue,
        };
    }
}