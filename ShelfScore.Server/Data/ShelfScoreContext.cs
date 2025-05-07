using Microsoft.EntityFrameworkCore;
using ShelfScore.Shared.Entities;

namespace ShelfScore.Server.Data;
public class ShelfScoreContext(DbContextOptions<ShelfScoreContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Author> Authors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- User Configuration ----------
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.DateTimeJoined).IsRequired();

            // User to Ratings relationship (one-to-many)
            entity.HasMany(u => u.Ratings)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Author Configuration ----------
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Ignore(e => e.FullName);  // Is a calculated property
        });

        // ---------- Book Configuration ----------
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ReleaseDate).IsRequired();
            entity.Property(e => e.RatingsSum).IsRequired();
            entity.Property(e => e.RatingsCount).IsRequired();
            entity.Property(e => e.RatingsAverage).IsRequired();

            // Book to Ratings relationship (one-to-many)
            entity.HasMany(b => b.Ratings)
                    .WithOne(r => r.Book)
                    .HasForeignKey(r => r.BookId)
                    .OnDelete(DeleteBehavior.Cascade);

            // Book to Authors relationship (many-to-many)
            entity.HasMany(b => b.Authors)
                    .WithMany(a => a.Books)
                    .UsingEntity(j => j.ToTable("BookAuthors"));
        });

        // ---------- Rating Configuration ----------
        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Score).IsRequired();
            entity.Property(e => e.TextReview).HasMaxLength(20000);
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}