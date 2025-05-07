using Microsoft.EntityFrameworkCore;
using ShelfScore.Server.Data;
using ShelfScore.Shared.Dtos;
using ShelfScore.Shared.Mappings; // Assuming your mapping extensions are in this namespace

namespace ShelfScore.Server.Endpoints;

public static class AuthorEndpoints
{
    public static IEndpointRouteBuilder MapAuthorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/authors").WithTags("Authors");

        group.MapGet("/", async (ShelfScoreContext db) =>
        {
            var authors = await db.Authors.ToListAsync();
            return Results.Ok(authors.Select(a => a.ToDto()));
        });

        group.MapGet("/{id:int}", async (int id, ShelfScoreContext db) =>
        {
            var author = await db.Authors.FindAsync(id);
            return author is not null ? Results.Ok(author.ToDto()) : Results.NotFound();
        });

        group.MapPost("/", async (CreateAuthorDto createAuthorDto, ShelfScoreContext db) =>
        {
            var author = createAuthorDto.ToEntity();
            db.Authors.Add(author);
            await db.SaveChangesAsync();
            return Results.Created($"/api/authors/{author.Id}", author.ToDto());
        });

        group.MapPut("/{id:int}", async (int id, UpdateAuthorDto updateAuthorDto, ShelfScoreContext db) =>
        {
            var author = await db.Authors.FindAsync(id);
            if (author is null)
                return Results.NotFound();

            author.ApplyUpdate(updateAuthorDto);
            await db.SaveChangesAsync();
            return Results.Ok(author.ToDto());
        });

        group.MapDelete("/{id:int}", async (int id, ShelfScoreContext db) =>
        {
            var author = await db.Authors.FindAsync(id);
            if (author is null)
                return Results.NotFound();

            db.Authors.Remove(author);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return app;
    }
}