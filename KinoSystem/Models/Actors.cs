using KinoSystem.Models.Utilities;

namespace KinoSystem.Models
{
    public class Actor
    {
        public int IdActor { get; set; }
        public string? Name { get; set; }
        public ActorType ActorType { get; set; }
        public List<Film> Films { get; set; }
    }
}
