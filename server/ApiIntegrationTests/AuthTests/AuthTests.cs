using API;
using DataAccess;
using DataAccess.Models;
using Generated;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Service.Security;
using Xunit.Abstractions;

namespace ApiInterationTests.AuthTests;


/// <summary>
/// All the tests are passing it is a problem with the service that is using emails, that will prevent the server to start 
/// </summary>
public class AuthTests : IClassFixture<ApiTestBase>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    private IServiceProvider ApplicationServices => _factory.Services;
    private HttpClient TestHttpClient { get; }
    private string TEST_PASS = "adminA1!";

    public AuthTests(ApiTestBase factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _factory = factory;
        TestHttpClient = _factory.CreateClient();
    }


    [Fact]
    public async Task TetUserLoginWithValidCredentials()
    {
        var testToken = "";
        try
        {
            using var scope = _factory.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = TestObjects.GetUser();

            var rolePlayer = "Player";

            await userManager.CreateAsync(user, TEST_PASS);
            if (!await userManager.IsInRoleAsync(user, rolePlayer))
            {
                await userManager.AddToRoleAsync(user, rolePlayer);
            }
            testToken = await tokenService.GetTokenAsync(user.Email!);
            var loginObject = new LoginRequest { Email = user.Email, Password =TEST_PASS };
            _testOutputHelper.WriteLine($"Request: {loginObject.Email}, {loginObject.Password}");
            var loginResponse = await new AuthClient(TestHttpClient).LoginAsync(loginObject);
            _testOutputHelper.WriteLine(testToken);
            _testOutputHelper.WriteLine("Nothing");
            _testOutputHelper.WriteLine(loginResponse.Result.Jwt);
            Assert.Equal(testToken, loginResponse.Result.Jwt);
        }
        catch (Exception ex)
        {
            _testOutputHelper.WriteLine($"Exception: {ex.Message}");
            _testOutputHelper.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw;
        }
    }


    [Fact]
    public async Task TetUserLoginWithInvalidPasswordIfReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = TestObjects.GetUser();
        var rolePlayer = "Player";
        await userManager.CreateAsync(user, TEST_PASS);
        if (!await userManager.IsInRoleAsync(user, rolePlayer))
        {
            await userManager.AddToRoleAsync(user, rolePlayer);
        }

        var loginObject = new LoginRequest { Email = user.Email, Password = "" };
        _testOutputHelper.WriteLine($"Request: {loginObject.Email}, {loginObject.Password}");

        try
        {
            var loginResponse = await new AuthClient(TestHttpClient).LoginAsync(loginObject);
            Assert.Fail("Expected a BadRequest response, but the request succeeded.");
        }
        catch (ApiException ex)
        {
            _testOutputHelper.WriteLine($"API Exception: {ex.Message}");
            Assert.Equal(400, ex.StatusCode);
        }
    }
    
    [Fact]
    public async Task TetUserLoginWithInvalidEmailIfReturnBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = TestObjects.GetUser();
        var rolePlayer = "Player";
        await userManager.CreateAsync(user, TEST_PASS);
        if (!await userManager.IsInRoleAsync(user, rolePlayer))
        {
            await userManager.AddToRoleAsync(user, rolePlayer);
        }
        var loginObject = new LoginRequest { Email = "", Password = TEST_PASS };
        try
        {
            var loginResponse = await new AuthClient(TestHttpClient).LoginAsync(loginObject);
            Assert.Fail("Expected a BadRequest response, but the request succeeded.");
        }
        catch (ApiException ex)
        {
            _testOutputHelper.WriteLine($"API Exception: {ex.Message}");
            Assert.Equal(400, ex.StatusCode);
        }
    }
}