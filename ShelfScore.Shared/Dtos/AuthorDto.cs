namespace ShelfScore.Shared.Dtos;


public record AuthorDto(
    int Id,
    string FirstName,
    string LastName,
    string FullName
);