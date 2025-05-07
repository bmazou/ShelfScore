namespace ShelfScore.Shared.Entities;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required DateOnly ReleaseDate { get; set; }    
    public int RatingsSum { get; set; }
    public int RatingsCount { get; set; }
    public float RatingsAverage { get; private set; }
    
    public ICollection<Author> Authors { get; set; } = [];
    public ICollection<Rating> Ratings { get; set; } = [];


    public void UpdateRatingsAverage() => RatingsAverage = RatingsCount != 0 ? (float)RatingsSum / RatingsCount : 0;
}