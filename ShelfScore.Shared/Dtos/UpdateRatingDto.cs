namespace ShelfScore.Shared.Dtos;


public record UpdateRatingDto(
    int Score,
    string? TextReview
    // UserId will get inferred from authentication
);