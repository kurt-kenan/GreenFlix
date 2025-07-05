using GreenFlix.Models;

public class WatchedMovie
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public Users User { get; set; }  // Navigation property

    public int MovieId { get; set; }
    public Movie Movie { get; set; }  // Navigation property

    public DateTime WatchedDate { get; set; }
}
