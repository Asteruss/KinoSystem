using KinoSystem.Models.Utilities;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace KinoSystem.Models.Database
{
    public class KinoDBContext:DbContext
    {
        public DbSet<Row> Rows { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Purhaces> Purhaces { get; set; }
        public KinoDBContext(DbContextOptions<KinoDBContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Row>().HasKey(r => r.IdRow);
            modelBuilder.Entity<Row>().Property(r => r.IdRow).ValueGeneratedOnAdd();

            modelBuilder.Entity<Seat>().HasKey(s => s.IdSeat);
            modelBuilder.Entity<Seat>().Property(r => r.IdSeat).ValueGeneratedOnAdd();

            modelBuilder.Entity<Hall>().HasKey(s => s.IdHall);
            modelBuilder.Entity<Hall>().Property(r => r.IdHall).ValueGeneratedOnAdd();

            modelBuilder.Entity<Person>().HasKey(p => p.IdPerson);
            modelBuilder.Entity<Person>().Property(p => p.IdPerson).ValueGeneratedOnAdd();
            modelBuilder.Entity<Person>().HasAlternateKey(p => p.Login);
            modelBuilder.Entity<Person>().Property(p => p.Login).IsRequired(true);
            modelBuilder.Entity<Person>().Property(p => p.Password).IsRequired(true);
            modelBuilder.Entity<Person>().Property(p => p.AccessRight).HasDefaultValue(AccessRight.Customer);


            modelBuilder.Entity<Film>().HasKey(q => q.IdFilm);
            modelBuilder.Entity<Film>().Property(q => q.IdFilm).ValueGeneratedOnAdd();
            modelBuilder.Entity<Film>().Property(q => q.InRental).HasDefaultValue(false);

            modelBuilder.Entity<Session>().HasKey(q => q.IdSesstion);
            modelBuilder.Entity<Session>().Property(q => q.IdSesstion).ValueGeneratedOnAdd();

            modelBuilder.Entity<Schedule>().HasKey(q => q.IdSchedule);
            modelBuilder.Entity<Schedule>().Property(q => q.IdSchedule).ValueGeneratedOnAdd();

            modelBuilder.Entity<Actor>().HasKey(q => q.IdActor);
            modelBuilder.Entity<Actor>().Property(q => q.IdActor).ValueGeneratedOnAdd();

            modelBuilder.Entity<Purhaces>().HasKey(q => q.IdPurchace);
            modelBuilder.Entity<Purhaces>().Property(q => q.IdPurchace).ValueGeneratedOnAdd();


            modelBuilder.Entity<Film>().HasMany(f => f.Actors).WithMany(a => a.Films).UsingEntity("FilmsAndActors");
            modelBuilder.Entity<Seat>().HasOne(r => r.Row).WithMany(s => s.Seats).HasForeignKey(s => s.IdRow).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Row>().HasOne(r => r.Hall).WithMany(h => h.Rows).HasForeignKey(r => r.IdHall).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Hall>().HasOne(h => h.Session).WithOne(s => s.Hall).HasForeignKey<Session>(s => s.IdHall).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Session>().HasOne(s => s.Film).WithMany(f => f.Sessions).HasForeignKey(s => s.IdFilm).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Schedule>().HasOne(s => s.Session).WithOne(s => s.Schedule).HasForeignKey<Schedule>(s => s.IdSchedule).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Seat>().HasOne(s => s.Person).WithMany(p => p.Seats).HasForeignKey(s => s.IdPerson).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Purhaces>().HasOne(p => p.Person).WithMany(p => p.Purhaces).HasForeignKey(p => p.IdPerson).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Purhaces>().HasOne(p => p.Session).WithMany(p => p.Purhaces).HasForeignKey(p => p.IdSession).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Person>().HasData(new Person[] {
                new Person(){IdPerson = 1, Name="sdf", Sername="sdf", MiddleName="sdf", Login="asd@mail.ru", Password="asd", AccessRight=AccessRight.Customer},
                new Person(){IdPerson = 2, Name="cas", Sername="cas", MiddleName="cas", Login="cas@mail.ru", Password="cas", AccessRight=AccessRight.Cashier},
                new Person(){IdPerson = 3, Name="admin", Sername="admin", MiddleName="admin", Login="admin@mail.ru", Password="admin", AccessRight=AccessRight.Administrator},
            });

            
        }

    }
}
