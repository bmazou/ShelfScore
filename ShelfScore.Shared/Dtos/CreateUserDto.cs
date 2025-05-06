namespace ShelfScore.Shared.Dtos;


public record CreateUserDto(
    string Username,
    string Email,
    string Password,    // Will be hashed by application
    string? FirstName,
    string? LastName
);