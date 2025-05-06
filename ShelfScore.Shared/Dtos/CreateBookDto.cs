namespace ShelfScore.Shared.Dtos;


public record CreateBookDto(
    string Title,
    DateOnly ReleaseDate,
    int AuthorId
);