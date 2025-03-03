using Common.ErrorMessages;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess;

public class AdminUserManagementRepository : IAdminUserManagementRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AdminUserManagementRepository> _logger;

    public AdminUserManagementRepository(AppDbContext context, UserManager<User> userManager, ILogger<AdminUserManagementRepository> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }
    
    public List<UserProfile> GetAllUsersDetails(int page, int pageSize, out int totalUsers)
    {
        totalUsers = _context.UserProfiles.Count(); 

        return _context.UserProfiles
            .Include(u => u.User)
            .OrderBy(u => u.UserName) 
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public async Task<UserProfile> AddUserProfileAsync(UserProfile userProfile)
    {
        await using (var transaction = await _context.Database.BeginTransactionAsync())
            try
            {
                if (userProfile == null)
                {
                    throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.InvalidUserProfile));
                }

                var balance = new Balance()
                {
                    UserId = userProfile.UserId,
                    Value = 0
                };
                _context.UserProfiles.Add(userProfile);
                _context.Balances.Add(balance);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return userProfile;
            }
            catch (ApplicationException e)
            {
                await transaction.RollbackAsync();
                _logger.Log(LogLevel.Warning, e.Message, e);
                throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.InvalidUserProfile), e);
            }
        
    }
    
    public async Task<UserProfile> UpdateUserProfile(UserProfile userProfile)
    {
        if (userProfile == null)
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.InvalidUserProfile));
        }

        var existingProfile = await _context.UserProfiles.FindAsync(userProfile.UserId);
        if (existingProfile == null)
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.UserNotFound));
        }

        existingProfile.UserName = userProfile.UserName;
        existingProfile.isactive = userProfile.isactive;

        _context.Entry(existingProfile).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return existingProfile;
    }
    
    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.UserNotFound));
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.FailedDeleteUser));
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (userProfile != null)
            {
                _context.UserProfiles.Remove(userProfile);
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return true;
        }
        catch(ApplicationException e)
        {
            await transaction.RollbackAsync();
            _logger.Log(LogLevel.Warning, e.Message, e);
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.FailedDeleteUser));
        }
    }
    
}