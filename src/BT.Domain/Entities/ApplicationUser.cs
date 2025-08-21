using Microsoft.AspNetCore.Identity;

namespace BT.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}".Trim();
    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}