namespace BT.API.DTOs;

public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public DateTime? TokenExpiry { get; set; }
    public UserInfo? User { get; set; }
}