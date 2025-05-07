using Microsoft.EntityFrameworkCore;
using ShelfScore.Server.Data;
using ShelfScore.Shared.Dtos;
using ShelfScore.Shared.Mappings;

namespace ShelfScore.Server.Endpoints;

public static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/books").WithTags("Books");

        group.MapGet("/", async (ShelfScoreContext db) =>
        {
            var books = await db.Books
                                .Include(b => b.Authors)
                                .ToListAsync();
            return Results.Ok(books.Select(b => b.ToDto()));
        });

        group.MapGet("/{id:int}", async (int id, ShelfScoreContext db) =>
        {
            var book = await db.Books
                               .Include(b => b.Authors)
                               .FirstOrDefaultAsync(b => b.Id == id);

            return book is not null ? Results.Ok(book.ToDto()) : Results.NotFound();
        });

        group.MapPost("/", async (CreateBookDto createBookDto, ShelfScoreContext db) =>
        {
            var authors = await db.Authors
                                  .Where(a => createBookDto.AuthorIds.Contains(a.Id))
                                  .ToListAsync();

            if (authors.Count != createBookDto.AuthorIds.Count)
                return Results.BadRequest("One or more author IDs are invalid.");

            var book = createBookDto.ToEntity(authors);
            db.Books.Add(book);
            await db.SaveChangesAsync();
            return Results.Created($"/api/books/{book.Id}", book.ToDto());
        });

        group.MapPut("/{id:int}", async (int id, UpdateBookDto updateBookDto, ShelfScoreContext db) =>
        {
            var book = await db.Books
                               .Include(b => b.Authors)
                               .FirstOrDefaultAsync(b => b.Id == id);
            if (book is null)
                return Results.NotFound();

            var updatedAuthors = await db.Authors
                                         .Where(a => updateBookDto.AuthorIds.Contains(a.Id))
                                         .ToListAsync();

            if (updatedAuthors.Count != updateBookDto.AuthorIds.Count)
                return Results.BadRequest("One or more author IDs for update are invalid.");

            book.ApplyUpdate(updateBookDto, updatedAuthors);
            await db.SaveChangesAsync();
            return Results.Ok(book.ToDto());
        });

        group.MapDelete("/{id:int}", async (int id, ShelfScoreContext db) =>
        {
            var book = await db.Books.FindAsync(id);
            if (book is null)
                return Results.NotFound();

            db.Books.Remove(book);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return app;
    }
}