using DataAccess;
using DataAccess.ConfigurationsRepository;

namespace Service;

public class DatabaseUpdateService (IDataBaseUpdateRepository update, ConfigurationRepository configurationRepository) : IDatabaseUpdateService
{
    public async Task UpdateGameState()
    { 
        await update.UpdateGameState();
    }
}