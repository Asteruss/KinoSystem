namespace KinoSystem.Models
{
    public class Session
    {
        public int IdSesstion { get; set; }
        public int IdFilm { get; set; }
        public int IdHall { get; set; }
        public int Price { get; set; }
        public Film? Film { get; set; }
        public Hall? Hall { get; set; }
        public Schedule? Schedule { get; set; }
        public List<Purhaces> Purhaces { get; set; } = new();
    }
}
