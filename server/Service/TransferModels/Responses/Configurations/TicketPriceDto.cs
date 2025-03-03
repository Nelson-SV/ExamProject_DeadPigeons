namespace Service.TransferModels.Responses.Configurations;

public class TicketPriceDto
{
    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

    public int NumberOfFields { get; set; }

    public int Price { get; set; }
}