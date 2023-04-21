namespace KinoSystem.Models.Utilities
{
    public enum HallType
    {
        Big = 1,
        Middle = 3,
        Small = 2
    }
    public enum AccessRight
    {
        Customer,
        Cashier,
        Administrator
    }
    public enum ActorType
    {
        Actor,
        Writer,
        Producer,
        Operator,
        Designer,
        Composer
    }
    public class Utililies
    {
        public static Hall GetHall(HallType type)
        {
            return new Hall();
        }
    }
}
