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
using Xunit.Abstractions;

namespace ApiInterationTests.CustomerTests;

public class UserTicketsHistoryTests : IClassFixture<ApiTestBase>
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient TestHttpClient { get; }

    public UserTicketsHistoryTests(ApiTestBase factory)
    {
        _factory = factory;
        TestHttpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task GetUserGameTicketsHistory_ShouldReturnStatusCodeOK()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        
        var user = await ctx.Users.Include(u => u.Roles).FirstOrDefaultAsync();
        var game = await ctx.Games.FirstOrDefaultAsync();
        var token = await tokenService.GetTokenAsync(user.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var gameTicket = new GameTicket
        {
            GUID = Guid.NewGuid(),
            PurchaseDate = DateTime.UtcNow,
            Sequence = [1, 2, 3, 4],
            UserId = user.Id,
            PriceValue = 20,
            GameId = game.Guid,
        };
        
        ctx.GameTickets.Add(gameTicket);
        await ctx.SaveChangesAsync();
        var response = await new UserClient(TestHttpClient).GetUserTicketsHistoryAsync(user.Id, 1, 10);
        
        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
    }
}