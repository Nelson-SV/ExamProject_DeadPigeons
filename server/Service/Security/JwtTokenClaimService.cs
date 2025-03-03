using System.Security.Claims;

using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Service.Security;

public class JwtTokenClaimService : ITokenClaimsService
{
    public const string SignatureAlgorithm = SecurityAlgorithms.HmacSha512;

    private readonly AppOptions _options;
    private readonly UserManager<User> _userManager;

    public JwtTokenClaimService(IOptions<AppOptions> options, UserManager<User> userManager)
    {
        _options = options.Value;
        _userManager = userManager;
    }
    public JwtTokenClaimService()
    {
    }
    
    
    

    // public async Task <string>GenerateToken(User user)
    // {
    //     var tokenT = "dfKDL0Rq26AEQhdHBcQkOvMNjj9S8/thdKhTVzm3UDWXfJ0gePCuWyf48VK9/hk1ID4VHqZjXpYhinms1r+Khg==";
    //     var roles = new List<string>();
    //     foreach (var rol in user.Roles)
    //     {
    //         roles.Add(rol.Name!);
    //     }
    //     var address = "http://localhost:5000";
    //     var key = Convert.FromBase64String(tokenT);
    //     var tokenDescriptor = new SecurityTokenDescriptor
    //     {
    //         SigningCredentials = new SigningCredentials(
    //             new SymmetricSecurityKey(key),
    //             SignatureAlgorithm
    //         ),
    //         Subject = new ClaimsIdentity(user.ToClaims(roles)),
    //         Expires = DateTime.UtcNow.AddDays(7),
    //         Issuer = address,
    //         Audience = address
    //     };
    //     var tokenHandler = new JsonWebTokenHandler();
    //     var token = tokenHandler.CreateToken(tokenDescriptor);
    //     return token;
    // }

    public async Task<string> GetTokenAsync(string userEmail)
    {
        var user = await _userManager.FindByEmailAsync(userEmail)
                   ?? throw new Exception("Could not find user");
        var roles = await _userManager.GetRolesAsync(user);

        var key = Convert.FromBase64String(_options.JwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SignatureAlgorithm
            ),
            Subject = new ClaimsIdentity(user.ToClaims(roles)),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _options.Address,
            Audience = _options.Address
        };
        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return token;
    }

    public static TokenValidationParameters ValidationParameters(AppOptions options)
    {
        var key = Convert.FromBase64String(options.JwtSecret);
        return new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidAlgorithms = [SignatureAlgorithm],

            // These are important when we are validating tokens from a
            // different system
            ValidIssuer = options.Address,
            ValidAudience = options.Address,

            // Set to 0 when validating on the same system that created the token
            ClockSkew = TimeSpan.FromSeconds(0),

            // Default value is true already.
            // They are just set here to emphasis the importance.
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };
    }
}