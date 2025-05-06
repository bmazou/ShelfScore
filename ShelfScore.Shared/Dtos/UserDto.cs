namespace ShelfScore.Shared.Dtos;


public record UserDto(
    int Id,
    string Username,
    string Email,
    string? FirstName,
    string? LastName,
    DateTime DateTimeJoined
);