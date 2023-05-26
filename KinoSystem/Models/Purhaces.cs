namespace KinoSystem.Models
{
    public class Purhaces
    {
        public int IdPurchace { get; set; }
        public DateTime? PurchaseTime { get; set; }
        public int? IdSession { get; set; }
        public int? IdPerson { get; set; }
        public Session Session { get; set; }
        public Person Person { get; set; }
    }
}
