using KinoSystem.Models.Database;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections.Generic;

namespace KinoSystem.Models.Utilities
{
    public enum HallType
    {
        Big = 1,
        Middle = 2,
        Small = 3
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
        public static List<Film> Get5Movies(KinoDBContext db) =>
           db.Films.Take(5).ToList();
        
        public async static Task<Film?> GetMovieByIdAsync(KinoDBContext db, int idMovie) =>(db.Films.Any(m => m.IdFilm == idMovie))? await db.Films.Where(m => m.IdFilm == idMovie).FirstAsync(): null;
        public static Hall GetHall(HallType type)
        {
            int rowCount = (type == HallType.Big) ? 25 : (type == HallType.Middle) ? 20 : 15;
            int seatCount = (type == HallType.Big) ? 30 : (type == HallType.Middle) ? 25 : 20;
            string hallName = (type == HallType.Big) ? "Большой" : (type == HallType.Middle) ? "Средний" : "Маленький";
            Hall hall = new Hall();
            hall.NameHall = hallName;
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
        public async static Task<List<Purhaces>> GetLastPurchacesAsync(KinoDBContext db, int count) =>
            await db.Purhaces.Include(p => p.Person).Include(p => p.Session).ThenInclude(s => s.Film).ToListAsync();
        public async static Task<Person> GetPersonByIdAsync(KinoDBContext db, int idPerson) =>
            await db.People.Where(p => p.IdPerson == idPerson).FirstOrDefaultAsync();
        public async static Task<List<Seat>> GetSeatsByIdsAsync(KinoDBContext db, List<int> ids)=>
            await db.Seats.Include(s => s.Person).Where(s => ids.Contains(s.IdSeat)==true).ToListAsync();
        public async static Task<Session?> GetSessionByIdAsync(KinoDBContext db, int idSession) =>
            await db.Sessions.Include(s => s.Film).Include(s => s.Hall).Include(s => s.Hall.Rows).Include(s => s.Hall.Rows).ThenInclude(r => r.Seats).ThenInclude(s => s.Person).Where(s => s.IdSesstion == idSession).FirstOrDefaultAsync();
        public async static Task<List<Schedule?>> GetSessionsByDateAsync(KinoDBContext db, DateTime date) =>
            await db.Schedules.Include(s => s.Session).Include(s => s.Session.Film).Include(s => s.Session.Hall).Where(s => s.Start.Value.Year == date.Year && s.Start.Value.Month == date.Month && s.Start.Value.Day == date.Day).ToListAsync();
        public async static Task<List<Film?>> GetRentalMoviesAsync(KinoDBContext db) =>
            await db.Films.Where(f => f.InRental == true).ToListAsync();
        
        public async static void LoadMovies(KinoDBContext db)
        {
            if (db.Films.Count() > 0)
                return;

            using (var stream = new StreamReader("C:\\Users\\Artem\\Desktop\\python\\movies.txt"))
            {
                var data = stream.ReadToEnd();
                var data_lines = data.Split("|||");
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
