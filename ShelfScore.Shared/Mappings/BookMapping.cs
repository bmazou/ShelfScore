using ShelfScore.Shared.Dtos;
using ShelfScore.Shared.Entities;

namespace ShelfScore.Shared.Mappings;

public static class BookMapping
{
    public static BookDto ToDto(this Book book)
    {
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(book.Author);

        return new BookDto(
            book.Id,
            book.Title,
            book.ReleaseDate,
            book.RatingsCount,
            book.RatingsAverage,
            book.Author.ToDto()
        );
    }

    public static BookSummaryDto ToSummaryDto(this Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        return new BookSummaryDto(
            book.Id,
            book.Title
        );
    }

    public static Book ToEntity(this CreateBookDto createBookDto, Author author)
    {
        ArgumentNullException.ThrowIfNull(createBookDto);
        ArgumentNullException.ThrowIfNull(author);

        if (createBookDto.AuthorId != author.Id)
            throw new ArgumentException(
                $"Mismatched AuthorId. Expected {author.Id}, but got {createBookDto.AuthorId}.",
                nameof(author));

        return new Book {
            Title = createBookDto.Title,
            ReleaseDate = createBookDto.ReleaseDate,
            AuthorId = createBookDto.AuthorId,
            Author = author
            // NOTE: RatingsSum, RatingsCount, RatingsAverage are initialized to default values (0)
        };
    }

    public static void ApplyUpdate(this Book book, UpdateBookDto updateBookDto)
    {
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(updateBookDto);

        book.Title = updateBookDto.Title;
        book.ReleaseDate = updateBookDto.ReleaseDate;
        book.AuthorId = updateBookDto.AuthorId;
    }
}