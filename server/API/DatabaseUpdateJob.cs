using Service;

namespace Common;
using Quartz;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class DatabaseUpdateJob  : IJob
{
    private readonly IDatabaseUpdateService _databaseUpdateService;
    
    public DatabaseUpdateJob(IDatabaseUpdateService databaseUpdateService)
    {
        _databaseUpdateService = databaseUpdateService;
    }

    /* The method will be executed when the job runs in program.cs */
    public async Task Execute(IJobExecutionContext context)
    {
        await _databaseUpdateService.UpdateGameState();
    }
}