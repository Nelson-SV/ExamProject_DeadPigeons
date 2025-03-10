using DataAccess;
using PgCtx;
using Service;

namespace API.Extensions;

public static class PgCtxExtensions
{
    public static WebApplicationBuilder AddPgContainer(this WebApplicationBuilder builder)
    {
        var appOptions = builder.Configuration.GetSection(nameof(AppOptions)).Get<AppOptions>();
        if (appOptions.RunInTestContainer)
        {
            var pg = new PgCtxSetup<AppDbContext>();
            builder.Configuration[nameof(AppOptions) + ":" + nameof(AppOptions.DbConnectionString)] =
                pg._postgres.GetConnectionString();
        }

        return builder;
    }
}