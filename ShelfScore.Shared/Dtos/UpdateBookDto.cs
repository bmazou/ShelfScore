namespace ShelfScore.Shared.Dtos;


public record UpdateBookDto(
    string Title,
    DateOnly ReleaseDate,
    ICollection<int> AuthorIds
);
