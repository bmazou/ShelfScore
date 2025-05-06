using ShelfScore.Shared.Dtos;
using ShelfScore.Shared.Entities;

namespace ShelfScore.Shared.Mappings;

public static class AuthorMapping
{
    public static AuthorDto ToDto(this Author author)
    {
        ArgumentNullException.ThrowIfNull(author);

        return new AuthorDto(
            author.Id,
            author.FirstName,
            author.LastName,
            author.FullName
        );
    }

    public static Author ToEntity(this CreateAuthorDto createAuthorDto)
    {
        ArgumentNullException.ThrowIfNull(createAuthorDto);

        return new Author {
            FirstName = createAuthorDto.FirstName,
            LastName = createAuthorDto.LastName
        };
    }

    public static void ApplyUpdate(this Author author, UpdateAuthorDto updateAuthorDto)
    {
        ArgumentNullException.ThrowIfNull(author);
        ArgumentNullException.ThrowIfNull(updateAuthorDto);

        author.FirstName = updateAuthorDto.FirstName;
        author.LastName = updateAuthorDto.LastName;
    }
}