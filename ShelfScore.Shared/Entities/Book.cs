namespace ShelfScore.Shared.Entities;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public DateOnly ReleaseDate { get; set; }    
    public int RatingsSum { get; set; }
    public int RatingsCount { get; set; }
    public float RatingsAverage { get; private set; }

    public int AuthorId { get; set; }
    public required Author Author { get; set; } 
    
    public ICollection<Rating> Ratings { get; set; } = [];


    public void UpdateRatingsAverage() => RatingsAverage = RatingsCount != 0 ? (float)RatingsSum / RatingsCount : 0;
}