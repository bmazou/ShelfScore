using Microsoft.EntityFrameworkCore;
using ShelfScore.Server.Data;
using ShelfScore.Shared.Dtos;
using ShelfScore.Shared.Mappings;
using System.Security.Claims;

namespace ShelfScore.Server.Endpoints;

public static class RatingEndpoints
{
    public static IEndpointRouteBuilder MapRatingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ratings").WithTags("Ratings");
        
        group.MapGet("/", async (ShelfScoreContext db) =>
        {
            var ratings = await db.Ratings
                .Include(r => r.User)
                .Include(r => r.Book)
                .ToListAsync();
            return Results.Ok(ratings.Select(r => r.ToDto()));
        });

        group.MapGet("/{id:int}", async (int id, ShelfScoreContext db) =>
        {
            var rating = await db.Ratings
                .Include(r => r.User)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == id);

            return rating is not null ? Results.Ok(rating.ToDto()) : Results.NotFound();
        });

        group.MapPost("/", async (HttpContext http, CreateRatingDto createRatingDto, ShelfScoreContext db) =>
        {
            string userIdClaim = "1";      // Dummy value
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Results.Unauthorized();
            if (!int.TryParse(userIdClaim, out int userId))
                return Results.BadRequest("Invalid user claim.");

            var user = await db.Users.FindAsync(userId);
            if (user is null)
                return Results.BadRequest($"User with ID {userId} not found. Ensure you are logged in.");

            var book = await db.Books.FindAsync(createRatingDto.BookId);
            if (book is null)
                return Results.NotFound($"Book with ID {createRatingDto.BookId} not found.");

            // Check if this user has already rated this book
            var existingRatingForUserAndBook = await db.Ratings
                .FirstOrDefaultAsync(r => r.BookId == createRatingDto.BookId && r.UserId == user.Id);
            if (existingRatingForUserAndBook is not null)
                return Results.Conflict(new {
                    message = $"User has already rated this book. To change the rating, please update the existing rating (ID: {existingRatingForUserAndBook.Id}).",
                    ratingId = existingRatingForUserAndBook.Id
                });

            var rating = createRatingDto.ToEntity(user, book);
            db.Ratings.Add(rating);

            book.RatingsSum += rating.Score;
            book.RatingsCount++;
            book.UpdateRatingsAverage();

            await db.SaveChangesAsync();
            return Results.Created($"/api/ratings/{rating.Id}", rating.ToDto());
        });
        // .RequireAuthorization();

        group.MapPut("/{id:int}", async (int id, UpdateRatingDto updateRatingDto, ShelfScoreContext db) =>
        {
            var rating = await db.Ratings
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rating is null)
                return Results.NotFound();

            // TODO: Implement ownership check

            var book = rating.Book;
            if (book is null)
                return Results.Problem($"Associated book for rating ID {id} not found. Cannot update book aggregates.");

            int oldScore = rating.Score;
            rating.ApplyUpdate(updateRatingDto);

            book.RatingsSum = book.RatingsSum - oldScore + rating.Score;
            book.UpdateRatingsAverage();

            await db.SaveChangesAsync();
            await db.Entry(rating).Reference(r => r.User).LoadAsync();
            return Results.Ok(rating.ToDto());
        });
        // .RequireAuthorization();


        group.MapDelete("/{id:int}", async (int id, ShelfScoreContext db) =>
        {
            var rating = await db.Ratings
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rating is null)
                return Results.NotFound();

            // TODO: Implement ownership check

            var book = rating.Book;
            if (book is not null) {
                book.RatingsSum -= rating.Score;
                book.RatingsCount--;
                book.UpdateRatingsAverage();
            }
            else {
                // Note: Should never happen
                Console.WriteLine($"Warning: Deleting rating with ID {id}, but its associated book not found.");
            }

            db.Ratings.Remove(rating);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
        // .RequireAuthorization();

        return app;
    }
}