using BT.API.Models.Responses.Users;

namespace BT.API.Models.Responses.Auth;

public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public DateTime? TokenExpiry { get; set; }
    public UserResponse? User { get; set; }
}