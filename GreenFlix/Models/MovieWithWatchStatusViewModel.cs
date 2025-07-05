namespace GreenFlix.Models
{
    public class MovieWithWatchStatusViewModel
    {
        public Movie Movie { get; set; }
        public bool IsWatched { get; set; }
        public bool IsFavorited { get; set; } // 🔥 EKLENDİ

        public List<MovieComment> Comments { get; set; }

    }
}
