using System.Net.Http.Headers;
using API;
using DataAccess;
using DataAccess.Models;
using Generated;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Service.Security;
using Xunit.Abstractions;

namespace ApiInterationTests.GameHandlingTests;

public class GameHandlingTests : IClassFixture<ApiTestBase>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    private IServiceProvider ApplicationServices => _factory.Services;
    private HttpClient TestHttpClient { get; }
    private string TEST_PASS = "adminA1!";

    public GameHandlingTests(ApiTestBase factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _factory = factory;
        TestHttpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task TestRetrieveCurrentGameThatHasNoExtractedNumbers()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = TestObjects.GetUser();
        await userManager.CreateAsync(user, TEST_PASS);
        await userManager.AddToRoleAsync(user, "Admin");
        var game = TestObjects.GetGameInProgress();
        ctx.Games.Add(game);
        await ctx.SaveChangesAsync();
        var token = await tokenService.GetTokenAsync(user.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var request = await new GameHandlerClient(TestHttpClient).GetCurrentGamInfoAsync();
        Assert.Equivalent(game.Guid, request.Result.Guid);
        ClearDatabase(ctx);
    }

    [Fact]
    public async Task TestIfPlayerCanAccessAdminMethod()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = TestObjects.GetUser();
        await userManager.CreateAsync(user, TEST_PASS);
        await userManager.AddToRoleAsync(user, "Player");
        var game = TestObjects.GetGameInProgress();
        ctx.Games.Add(game);
        await ctx.SaveChangesAsync();
        var token = await tokenService.GetTokenAsync(user.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        try
        {
            var request = await new GameHandlerClient(TestHttpClient).GetCurrentGamInfoAsync();
            Assert.Fail("Expected a BadRequest response, but the request succeeded.");
        }
        catch (ApiException ex)
        {
            _testOutputHelper.WriteLine($"API Exception: {ex.Message}");
            Assert.Equal(403, ex.StatusCode);
        }
        ClearDatabase(ctx);
    }

    
    [Fact]
    public async Task TestInsertingTheWinningSequenceInsertion()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = TestObjects.GetUser();
        await userManager.CreateAsync(user, TEST_PASS);
        await userManager.AddToRoleAsync(user, "Admin");
        var game = TestObjects.GetGameInProgress();
        ctx.Games.Add(game);
        await ctx.SaveChangesAsync();
        var token = await tokenService.GetTokenAsync(user.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var request = await new GameHandlerClient(TestHttpClient).SetWinningNumbersAsync(new WinningSequenceDto()
            { WinningSequence = [1, 2, 3], GameId = game.Guid });
        
        Assert.True(request.Result.Registered);
        ClearDatabase(ctx);
    }
    
    [Fact]
    public async Task TestRetrieveWiningPlayersForAGameAfterTheWinningSequenceWasSet()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user1 = TestObjects.GetUser();
        var user2 = TestObjects.GetUser();
        var userAdmin = TestObjects.GetUser();
        await userManager.CreateAsync(user1, TEST_PASS);
        await userManager.AddToRoleAsync(user1, "Player");
        await userManager.CreateAsync(user2, TEST_PASS);
        await userManager.AddToRoleAsync(user2, "Player");
        await userManager.CreateAsync(userAdmin,TEST_PASS);
        await userManager.AddToRoleAsync(userAdmin,"Admin");
        await ctx.SaveChangesAsync();
        var game = TestObjects.GetGameInProgress();
        ctx.Games.Add(game);
        ctx.Balances.AddRange(new Balance(){UserId =user1.Id,Value=600 });
        ctx.Balances.AddRange(new Balance(){UserId =userAdmin.Id,Value=600 });
        var firstTicket = TestObjects.CreateGameTicket(1,game.Guid,user1.Id,[1,2,3,4,5,6]);
        var secondTicket = TestObjects.CreateGameTicket(2, game.Guid,user2.Id, [1, 6, 7, 8]);
        var thirdTicket = TestObjects.CreateGameTicket(3, game.Guid,userAdmin.Id, [1, 2, 3]);
        ctx.GameTickets.AddRange(firstTicket, secondTicket, thirdTicket);
        await ctx.SaveChangesAsync();
        var pagination = new Pagination()
        {
            CurrentPageItems = 10,
            CurrentPage = 1,
            NextPage = 2,
            HasNext = false,
            TotalItems = 10
        };
    var requestData = new WinningPlayersRequestDto()
    {
        GameId =new GameIdDto{ Guid = game.Guid },
        Pagination = pagination,
        WinningSequence = [1,2,3]
    };
    var token = await tokenService.GetTokenAsync(userAdmin.Email!);
    TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    var requestSetWinning = await new GameHandlerClient(TestHttpClient).SetWinningNumbersAsync(new WinningSequenceDto()
        { WinningSequence = [1, 2, 3], GameId = game.Guid });
    _testOutputHelper.WriteLine(requestSetWinning.Result.Message);
    var request =await new GameHandlerClient(TestHttpClient).GetWinningPlayersAsync(requestData);
    Assert.Equal(2,request.Result.WinningPlayers.Count);
    }

    
    private void ClearDatabase(AppDbContext ctx)
    {
        ctx.Games.RemoveRange(ctx.Games);
        ctx.Users.RemoveRange(ctx.Users);
        ctx.SaveChanges();
    }
    
}