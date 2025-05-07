namespace ShelfScore.Shared.Dtos;


public record CreateBookDto(
    string Title,
    DateOnly ReleaseDate,
    ICollection<int> AuthorIds
);