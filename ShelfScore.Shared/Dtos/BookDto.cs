namespace ShelfScore.Shared.Dtos;


public record BookDto(
    int Id,
    string Title,
    DateOnly ReleaseDate,
    int RatingsCount,
    float RatingsAverage,
    ICollection<AuthorDto> Authors
);