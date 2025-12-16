namespace DotNet8.WebApi.Entities;

public class Account : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

