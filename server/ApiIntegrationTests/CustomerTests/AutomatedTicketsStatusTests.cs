using System.Net;
using System.Net.Http.Headers;
using DataAccess;
using API;
using DataAccess.Models;
using Generated;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Service.Security;

namespace ApiInterationTests.CustomerTests;

public class AutomatedTicketsStatusTests : IClassFixture<ApiTestBase>
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient TestHttpClient { get; }
    
    public AutomatedTicketsStatusTests(ApiTestBase factory)
    {
        _factory = factory;
        TestHttpClient = _factory.CreateClient();
    }
    
    [Fact]
    public async Task DeleteAutomatedTicket_ShouldReturnSuccess_WhenTicketIsBoughtForCurrentWeek()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();

        var user = await ctx.Users.Include(u => u.Roles).FirstOrDefaultAsync();
        var activeGame = await ctx.Games.FirstOrDefaultAsync(g => g.Status);
        var token = await tokenService.GetTokenAsync(user.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var ticket = new GameTicket
        {
            GUID = Guid.NewGuid(),
            PurchaseDate = DateTime.UtcNow,
            Sequence = [1,2,3,4],
            UserId = user.Id,
            PriceValue = 20,
            GameId = activeGame.Guid
        };
        ctx.GameTickets.Add(ticket);
        await ctx.SaveChangesAsync();
        
        var gameTicket = new AutomatedTicket
        {
            Guid = ticket.GUID,
            UserId = user.Id,
            Sequence = ticket.Sequence,
            PurchaseDate = ticket.PurchaseDate,
            IsActive = true,
            PriceValue = ticket.PriceValue
            
        };
        ctx.AutomatedTickets.Add(gameTicket);
        await ctx.SaveChangesAsync();

        var request = new AutomatedTicketsDto { Guid = ticket.GUID, UserId = user.Id};
        var response = await new PlayClient(TestHttpClient).DeleteAutomatedTicketAsync(request);

        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAutomatedTicketStatus_ShouldActivateTicketAndCreateNewEntryForCurrentWeek()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        var user = await ctx.Users.Include(u => u.Roles).FirstOrDefaultAsync();
        var token = await tokenService.GetTokenAsync(user.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var activeGame = new Game
        {
            Guid = Guid.NewGuid().ToString(),
            Status = true,
            StartDate = DateTime.UtcNow
        };
        ctx.Games.Add(activeGame);
        await ctx.SaveChangesAsync();

        var ticket = new AutomatedTicket
        {
            Guid = Guid.NewGuid(),
            UserId = user.Id,
            IsActive = false,
            PurchaseDate = DateTime.UtcNow.AddDays(1),
            Sequence = [1,2,3,4],
            PriceValue = 20
        };
        ctx.AutomatedTickets.Add(ticket);
        await ctx.SaveChangesAsync();

        var request = new UpdateAutomatedTicketStatusRequest
        {
            AutomatedTicket = new AutomatedTicketsDto { Guid = ticket.Guid, UserId = user.Id},
            IsActive = true
        };

        var response = await new PlayClient(TestHttpClient).UpdateAutomatedTicketStatusAsync(request);

        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
    }
    
    
    [Fact]
    public async Task DeleteAutomatedTicket_ShouldReturnSuccess_WhenNoCurrentWeekPurchase()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        var user = await ctx.Users.Include(u => u.Roles).FirstOrDefaultAsync();
        var token = await tokenService.GetTokenAsync(user.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var ticket = new AutomatedTicket
        {
            Guid = Guid.NewGuid(),
            UserId = user.Id,
            IsActive = true,
            PurchaseDate = DateTime.UtcNow,
            Sequence = [1,2,3,4],
            PriceValue = 20
        };
        ctx.AutomatedTickets.Add(ticket);
        await ctx.SaveChangesAsync();
        
        var request = new AutomatedTicketsDto { Guid = ticket.Guid, UserId = user.Id};
        var response = await new PlayClient(TestHttpClient).DeleteAutomatedTicketAsync(request);

        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
    }

}