namespace KinoSystem.Models
{
    public class Schedule
    {
        public int IdSchedule { get; set; }
        public int IdSession { get; set; }
        public Session? Session { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
