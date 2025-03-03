using DataAccess.ConfigurationsRepository;

namespace Service.TransferModels.Responses.Configurations;

public class ConfigurationService
{
    private ConfigurationRepository _configurationRepo;

    public ConfigurationService(ConfigurationRepository configurationRepository)
    {
        _configurationRepo = configurationRepository;
    }

    public async Task<Dictionary<int, TicketPriceDto>> GetTicketPrices()
    {
        var TicketPrices = new Dictionary<int, TicketPriceDto>();
        var readPrices = await _configurationRepo.GetTicketPrices();
        foreach (var key in readPrices.Keys)
        {
            var tiketRepo = readPrices[key];
            var ticketPrice = new TicketPriceDto
            {
                Created = tiketRepo.Created,
                Updated = tiketRepo.Updated,
                NumberOfFields = tiketRepo.NumberOfFields,
                Price = tiketRepo.Price
            };

            TicketPrices.Add(key, ticketPrice);
        }
        return TicketPrices;
    }


    public async Task<List<TopUpValueDto>> GetTopUpValue()
    {
        var tickets = await _configurationRepo.GetTopUpValues();
        var topUpValues = tickets.Select((e) =>
        {
            return new TopUpValueDto
            {
                TopUpValue = e.TopUpVal
            };
        }).ToList();
        return topUpValues;
    }
}