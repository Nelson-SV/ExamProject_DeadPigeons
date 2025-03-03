namespace Service.TransferModels.Requests;

public record PasswordResetRequest(string Email, string Token, string Password);