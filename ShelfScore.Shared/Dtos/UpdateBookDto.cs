namespace ShelfScore.Shared.Dtos;


public record UpdateBookDto(
    string Title,
    DateOnly ReleaseDate,
    int AuthorId
);
