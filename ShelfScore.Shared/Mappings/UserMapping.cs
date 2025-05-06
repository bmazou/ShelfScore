using ShelfScore.Shared.Dtos;
using ShelfScore.Shared.Entities;

namespace ShelfScore.Shared.Mappings;

public static class UserMapping
{
    public static UserDto ToDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName,
            user.DateTimeJoined
        );
    }

    public static UserSummaryDto ToSummaryDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserSummaryDto(
            user.Id,
            user.Username
        );
    }

    public static User ToEntity(this CreateUserDto createUserDto)
    {
        ArgumentNullException.ThrowIfNull(createUserDto);

        return new User {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            PasswordHash = string.Empty,        // Placeholder, will be hashed later
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName
        };
    }

    public static void ApplyUpdate(this User user, UpdateUserDto updateUserDto)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(updateUserDto);

        user.Email = updateUserDto.Email;
        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
    }
}