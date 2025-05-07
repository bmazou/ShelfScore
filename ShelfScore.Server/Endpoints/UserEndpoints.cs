using Microsoft.EntityFrameworkCore;
using ShelfScore.Server.Data;
using ShelfScore.Shared.Dtos;
using ShelfScore.Shared.Mappings;
using System.Security.Cryptography; 
using System.Text; 

namespace ShelfScore.Server.Endpoints;

public static class UserEndpoints
{
    private const int SaltSize = 16; // 128 bit
    private const int HashSize = 32; // 256 bit
    private const int Iterations = 100_000;

    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/", async (ShelfScoreContext db) =>
        {
            var users = await db.Users.ToListAsync();
            return Results.Ok(users.Select(u => u.ToDto()));
        });

        group.MapGet("/{id:int}", async (int id, ShelfScoreContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            return user is not null ? Results.Ok(user.ToDto()) : Results.NotFound();
        });

        group.MapPost("/", async (CreateUserDto createUserDto, ShelfScoreContext db) =>
        {
            if (await db.Users.AnyAsync(u => u.Username == createUserDto.Username))
                return Results.Conflict("Username already exists.");
            if (await db.Users.AnyAsync(u => u.Email == createUserDto.Email))
                return Results.Conflict("Email already exists.");

            var user = createUserDto.ToEntity();
            byte[] salt = GenerateSalt();
            user.PasswordSalt = Convert.ToBase64String(salt);
            user.PasswordHash = HashPassword(createUserDto.Password, salt);

            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Created($"/api/users/{user.Id}", user.ToDto());
        });

        group.MapPut("/{id:int}", async (int id, UpdateUserDto updateUserDto, ShelfScoreContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null)
                return Results.NotFound();

            bool isChangingMails = user.Email != updateUserDto.Email;
            if (isChangingMails && await db.Users.AnyAsync(u => u.Email == updateUserDto.Email && u.Id != id))
                 return Results.Conflict("Email already in use by another account.");

            user.ApplyUpdate(updateUserDto);
            await db.SaveChangesAsync();
            return Results.Ok(user.ToDto());
        });

        group.MapDelete("/{id:int}", async (int id, ShelfScoreContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null)
                return Results.NotFound();

            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return app;
    }

    private static byte[] GenerateSalt() =>
        RandomNumberGenerator.GetBytes(SaltSize);

    private static string HashPassword(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(HashSize);
        return Convert.ToHexStringLower(hash);
    }

    public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSaltBase64)
    {
        byte[] salt = Convert.FromBase64String(storedSaltBase64);
        string hashOfEnteredPassword = HashPassword(enteredPassword, salt);
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(hashOfEnteredPassword),
            Convert.FromHexString(storedHash)
        );
    }
}