using ShelfScore.Shared.Dtos;
using ShelfScore.Shared.Entities;

namespace ShelfScore.Shared.Mappings;

public static class BookMapping
{
    public static BookDto ToDto(this Book book)
    {
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(book.Authors);

        return new BookDto(
            book.Id,
            book.Title,
            book.ReleaseDate,
            book.RatingsCount,
            book.RatingsAverage,
            book.Authors.Select(a => a.ToDto()).ToList()
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

    private static void ValidateAuthorsAgainstDtoIds(IEnumerable<Author> authors, IEnumerable<int> authorIds) 
    {
        var authorIdSetFromDto = new HashSet<int>(authorIds);
        if (authors.Count() != authorIdSetFromDto.Count || !authors.All(a => authorIdSetFromDto.Contains(a.Id)))
            throw new ArgumentException("The provided authors collection does not match the AuthorIds in the DTO.");
    }

    public static Book ToEntity(this CreateBookDto createBookDto, IEnumerable<Author> authors)
    {
        ArgumentNullException.ThrowIfNull(createBookDto);
        ArgumentNullException.ThrowIfNull(createBookDto.AuthorIds);
        ArgumentNullException.ThrowIfNull(authors);
        

        ValidateAuthorsAgainstDtoIds(authors, createBookDto.AuthorIds);

        return new Book {
            Title = createBookDto.Title,
            ReleaseDate = createBookDto.ReleaseDate,
            Authors = authors.ToList()
            // NOTE: RatingsSum, RatingsCount, RatingsAverage are initialized to default values (0)
        };
    }


    public static void ApplyUpdate(this Book book, UpdateBookDto updateBookDto, IEnumerable<Author> updatedAuthors)
    {
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(updateBookDto);
        ArgumentNullException.ThrowIfNull(updateBookDto.AuthorIds);
        ArgumentNullException.ThrowIfNull(updatedAuthors);

        ValidateAuthorsAgainstDtoIds(updatedAuthors, updateBookDto.AuthorIds);

        book.Title = updateBookDto.Title;
        book.ReleaseDate = updateBookDto.ReleaseDate;
        book.Authors = updatedAuthors.ToList();
    }
}