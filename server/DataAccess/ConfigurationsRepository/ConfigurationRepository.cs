using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.ConfigurationsRepository;

public class ConfigurationRepository
{
    private AppDbContext _appDbContext;

    public ConfigurationRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Dictionary<int,TicketPrice>> GetTicketPrices()
    {
        var prices =  await _appDbContext.TicketPrices.ToDictionaryAsync((e)=>e.NumberOfFields);
        Console.WriteLine(prices);
        return prices;
    }

    public async Task<List<TopUpValue>> GetTopUpValues()
    {
        var topUpValues = await _appDbContext.TopUpValues.OrderBy((e)=>e.TopUpVal).ToListAsync();
        return topUpValues;
    }


}