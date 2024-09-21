namespace Identity.Persistence.Options;

public sealed record AdminOptions
{
    public required string UserName { get; init; } 
    public required string Email { get; init; } 
    public required string Password { get; init; }
}