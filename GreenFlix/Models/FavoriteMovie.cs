namespace GreenFlix.Models
{
    public class FavoriteMovie
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }
        public DateTime AddedDate { get; set; }
    }

}
