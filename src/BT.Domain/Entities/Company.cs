namespace BT.Domain.Entities;

public class Company : BaseEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<ApplicationUser> Members { get; set; } = new List<ApplicationUser>();
}