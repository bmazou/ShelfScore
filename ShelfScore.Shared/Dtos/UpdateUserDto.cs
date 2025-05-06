namespace ShelfScore.Shared.Dtos;


public record UpdateUserDto(
    string Email,
    string? FirstName,
    string? LastName
);