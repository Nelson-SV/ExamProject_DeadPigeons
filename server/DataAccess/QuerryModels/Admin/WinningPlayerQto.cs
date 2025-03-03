namespace DataAccess.QuerryModels.Admin;

//Todo delete if not used
public class WinningPlayerQto
{
    public string Id { get; set; } = null!;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public List<string>? WinningTicketsIds { get; set; }
}