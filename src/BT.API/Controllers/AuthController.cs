using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BT.API.Models.Requests.Auth;
using BT.API.Models.Responses.Auth;
using BT.API.Models.Responses.Users;
using BT.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BT.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsPrincipalFactory;

    public AuthController(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        IConfiguration configuration, 
        ILogger<AuthController> logger, 
        IUserClaimsPrincipalFactory<ApplicationUser> claimsPrincipalFactory)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
        _claimsPrincipalFactory = claimsPrincipalFactory;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Ok(new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid email or password." 
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Ok(new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid email or password." 
                });
            }

            var token = await GenerateJwtToken(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                TokenExpiry = DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
                User = new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    // CompanyId = user.CompanyId,
                    Roles = userRoles.ToList()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return StatusCode(500, new AuthResponse 
            { 
                Success = false, 
                Message = "An error occurred during login." 
            });
        }
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return Ok(new AuthResponse 
                { 
                    Success = false, 
                    Message = "User with this email already exists." 
                });
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Ok(new AuthResponse 
                { 
                    Success = false, 
                    Message = $"Registration failed: {errors}" 
                });
            }

            // TODO: Review default role
            await _userManager.AddToRoleAsync(user, "Developer");

            var token = await GenerateJwtToken(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Registration successful.",
                Token = token,
                TokenExpiry = DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
                User = new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    // CompanyId = user.CompanyId,
                    Roles = userRoles.ToList()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            return StatusCode(500, new AuthResponse 
            { 
                Success = false, 
                Message = "An error occurred during registration." 
            });
        }
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { success = true, message = "Logout successful." });
    }
    
    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var principal = await _claimsPrincipalFactory.CreateAsync(user);
        var claims = principal.Claims.ToList();

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes()),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int GetTokenExpiryMinutes()
    {
        return _configuration.GetValue<int>("Jwt:ExpirationMinutes", 60);
    }
}