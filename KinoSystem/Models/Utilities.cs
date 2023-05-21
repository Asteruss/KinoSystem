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
            int rowCount = (type == HallType.Big) ? 25 : (type == HallType.Middle) ? 20 : 15;
            int seatCount = (type == HallType.Big) ? 30 : (type == HallType.Middle) ? 25 : 20;
            Hall hall = new Hall();
            for (int i = 0; i < rowCount; i++)
            {
                Row row = new Row() { Hall = hall, Number = i + 1 };
                for (int j = 0; j < seatCount; j++)
                    row.Seats.Add(new Seat() { Row = row, Number = j + 1 });
                hall.Rows.Add(row);
            }
            Console.WriteLine();
            return hall;
        }
    }
}
