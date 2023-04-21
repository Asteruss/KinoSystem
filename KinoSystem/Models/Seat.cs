namespace KinoSystem.Models
{
    public class Seat
    {
        public int IdSeat { get; set; }
        public int Number { get; set; }
        public int IdRow { get; set; }
        public Row Row { get; set; }
        public int IdPerson { get; set; }
        public Person Person { get; set; }
    }
}
