﻿namespace Service.TransferModels.Requests;

public class ResetPasswordResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}