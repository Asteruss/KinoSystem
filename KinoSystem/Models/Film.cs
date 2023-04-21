namespace KinoSystem.Models
{
    public class Film
    {
        public int IdFilm { get; set; }
        public string? Title { get; set; }
        public int? AgeRating { get; set;}
        public int? Rating { get; set;}
        public int? MovieLength { get; set; }
        public int? ReleaseYear { get; set; }
        public string? UrlPoster { get; set; }
        public string? Genres { get; set; }
        public string? Description { get; set; }
        public List<Actor> Actors { get; set; }
        public List<Session> Sessions { get; set; } = new();
    }
}
