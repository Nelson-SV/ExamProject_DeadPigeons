using System.Net.Http.Headers;
using API;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PgCtx;
using Service;
using Service.Security;

namespace ApiInterationTests;

public class ApiTestBase : WebApplicationFactory<Program>
{
 
    public ApiTestBase()
    {
       
        // Initialize services
        PgCtxSetup = new PgCtxSetup<AppDbContext>();
        Environment.SetEnvironmentVariable(nameof(AppOptions) + ":" + nameof(AppOptions.DbConnectionString), PgCtxSetup._postgres.GetConnectionString());
   

    }

    /// <summary>
    ///     Data that will be populated before each test
    /// </summary>
    public async Task Seed(IServiceScope scope)
    {
        var scopedServices = scope.ServiceProvider;
        var ctx = scopedServices.GetRequiredService<AppDbContext>();
        var userManager = scopedServices.GetRequiredService<UserManager<User>>();
        var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();
        

        // Create roles if they don't exist
        var roleAdmin = "Admin";
        var rolePlayer = "Player";

        if (!await roleManager.RoleExistsAsync(roleAdmin))
        {
            await roleManager.CreateAsync(new IdentityRole(roleAdmin));
        }

        if (!await roleManager.RoleExistsAsync(rolePlayer))
        {
            await roleManager.CreateAsync(new IdentityRole(rolePlayer));
        }
        
        
        

        // Create user if it doesn't exist
        var user = TestObjects.GetUser(); // Assuming this returns an IdentityUser or similar
        var userInDb = await userManager.FindByEmailAsync(user.Email!);

        if (userInDb == null)
        {

            var createUserResult = await userManager.CreateAsync(user, "DefaultPassword123!");
            if (!createUserResult.Succeeded)
            {
                Console.WriteLine(
                    $"Error creating user: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
                return;
            }
        }
        
                      if (!await userManager.IsInRoleAsync(user, rolePlayer))
                      {
                          await userManager.AddToRoleAsync(user, rolePlayer);
                      }
          
                      if (!await userManager.IsInRoleAsync(user, roleAdmin))
                      {
                          await userManager.AddToRoleAsync(user, roleAdmin);
                      }  

            // Add ticket prices and game to the database
            var game = TestObjects.GetGame();
                        
            var ticketPrices = new[]
            {
                new TicketPrice
                {
                    Guid = Guid.NewGuid(), NumberOfFields = 5, Price = 20, Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                },
                new TicketPrice
                {
                    Guid = Guid.NewGuid(), NumberOfFields = 6, Price = 40, Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                },
                new TicketPrice
                {
                    Guid = Guid.NewGuid(), NumberOfFields = 7, Price = 80, Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                },
                new TicketPrice
                {
                    Guid = Guid.NewGuid(), NumberOfFields = 8, Price = 160, Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                }
            };
            //ctx.Users.Add(user);
            ctx.TicketPrices.AddRange(ticketPrices);
            ctx.Games.Add(game);
            await ctx.SaveChangesAsync();

            Console.WriteLine("Seeding completed.");
        }
    



    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseNpgsql(PgCtxSetup._postgres.GetConnectionString());
                opt.EnableSensitiveDataLogging(false);
                opt.LogTo(_ => { });
            });
        });
        //return base.CreateHost(builder);
        
        var host = builder.Build();

        using (var scope = host.Services.CreateScope())
        {
            Seed(scope).GetAwaiter().GetResult();
        }

        host.Start();
        return host;
    }

    #region properties

    public PgCtxSetup<AppDbContext> PgCtxSetup;

    public string UserJwt { get; set; } =
        "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWUsImlhdCI6MTUxNjIzOTAyMn0.0Bk7pFvb2zgnomw3gUNpoCNq9fEhAD-qrzD38eOjo4PN0PZwiZbcssGRuslR0KG9umsY1lB0MFCH54eRSficnQ";
    
    #endregion
}