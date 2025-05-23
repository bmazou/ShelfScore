namespace ShelfScore.Shared.Entities;
public class Author
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public ICollection<Book> Books { get; set; } = [];
}