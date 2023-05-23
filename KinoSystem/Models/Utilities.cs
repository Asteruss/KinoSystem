using KinoSystem.Models.Database;
using Microsoft.EntityFrameworkCore;
using System.IO;

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
        public static List<Film> Get5Movies(KinoDBContext db)
        {
            return db.Films.Take(5).ToList();
        }
        public async static Task<Film?> GetMovieByIdAsync(KinoDBContext db, int idMovie) =>(db.Films.Any(m => m.IdFilm == idMovie))? await db.Films.Where(m => m.IdFilm == idMovie).FirstAsync(): null;
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
        public async static void LoadMovies(KinoDBContext db)
        {
            if (db.Films.Count() > 0)
                return;

            using (var stream = new StreamReader("C:\\Users\\Artem\\Desktop\\python\\movies.txt"))
            {
                var data = stream.ReadToEnd();
                var data_lines = data.Split("\r\n");
                foreach (var line in data_lines)
                {
                    if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
                        continue;

                    var data_line = line.Split("|").ToList();
                    if (data_line.Count() < 8)
                        continue;
                    await db.Films.AddAsync(_getMovie(data_line));
                    await db.SaveChangesAsync();
                }

            }
        }
        private static int _tryint;
        private static Film _getMovie(List<string> args) => new Film()
        {
            Title = args[0],
            AgeRating = (int.TryParse(args[1], out _tryint) == false) ? 0 : int.Parse(args[1]),
            Rating =  double.Parse(args[2], System.Globalization.CultureInfo.InvariantCulture),
            MovieLength = (int.TryParse(args[3], out _tryint) == false) ? 0 : int.Parse(args[3]),
            ReleaseYear = int.Parse(args[4]),
            UrlPoster = args[5],
            Genres = args[6],
            Description = args[7]
        };
    }
}
