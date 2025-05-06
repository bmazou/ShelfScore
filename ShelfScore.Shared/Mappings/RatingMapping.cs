using ShelfScore.Shared.Dtos;
using ShelfScore.Shared.Entities;

namespace ShelfScore.Shared.Mappings;

public static class RatingMapping
{
    public static RatingDto ToDto(this Rating rating)
    {
        ArgumentNullException.ThrowIfNull(rating);
        ArgumentNullException.ThrowIfNull(rating.Person);
        ArgumentNullException.ThrowIfNull(rating.Book);

        return new RatingDto(
            rating.Id,
            rating.Score,
            rating.TextReview,
            rating.CreatedAt,
            rating.Person.ToSummaryDto(),
            rating.Book.ToSummaryDto()
        );
    }

    public static Rating ToEntity(this CreateRatingDto createRatingDto, User user, Book book)
    {
        ArgumentNullException.ThrowIfNull(createRatingDto);
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(book);

        if (createRatingDto.BookId != book.Id)
            throw new ArgumentException(
                $"Mismatched BookId. Expected {book.Id}, but got {createRatingDto.BookId}.",
                nameof(book));

        return new Rating {
            Score = createRatingDto.Score,
            TextReview = createRatingDto.TextReview,
            BookId = createRatingDto.BookId,
            Book = book,
            PersonId = user.Id,   // `createRatingDto` doesn't have `UserId`, so set it directly from `user`
            Person = user
        };
    }

    public static void ApplyUpdate(this Rating rating, UpdateRatingDto updateRatingDto)
    {
        ArgumentNullException.ThrowIfNull(rating);
        ArgumentNullException.ThrowIfNull(updateRatingDto);

        rating.Score = updateRatingDto.Score;
        rating.TextReview = updateRatingDto.TextReview;
    }
}