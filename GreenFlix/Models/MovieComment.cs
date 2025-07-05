namespace GreenFlix.Models
{
    public class MovieComment
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
