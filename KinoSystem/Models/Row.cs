namespace KinoSystem.Models
{
    public class Row
    {
        public int IdRow { get; set; }
        public int Number { get; set; }
        public int IdHall { get; set; }
        public Hall Hall { get; set; }
        public List<Seat> Seats { get; set; }

    }
}
