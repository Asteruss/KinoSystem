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
        public KinoDBContext(DbContextOptions<KinoDBContext> options) : base(options)
        {
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

            modelBuilder.Entity<Session>().HasKey(q => q.IdSesstion);
            modelBuilder.Entity<Session>().Property(q => q.IdSesstion).ValueGeneratedOnAdd();

            modelBuilder.Entity<Schedule>().HasKey(q => q.IdSchedule);
            modelBuilder.Entity<Schedule>().Property(q => q.IdSchedule).ValueGeneratedOnAdd();

            modelBuilder.Entity<Actor>().HasKey(q => q.IdActor);
            modelBuilder.Entity<Actor>().Property(q => q.IdActor).ValueGeneratedOnAdd();

            modelBuilder.Entity<Film>().HasMany(f => f.Actors).WithMany(a => a.Films).UsingEntity("FilmsAndActors");
            modelBuilder.Entity<Seat>().HasOne(r => r.Row).WithMany(s => s.Seats).HasForeignKey(s => s.IdRow).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Row>().HasOne(r => r.Hall).WithMany(h => h.Rows).HasForeignKey(r => r.IdHall).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Hall>().HasOne(h => h.Session).WithOne(s => s.Hall).HasForeignKey<Session>(s => s.IdHall).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Session>().HasOne(s => s.Film).WithMany(f => f.Sessions).HasForeignKey(s => s.IdFilm).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Schedule>().HasOne(s => s.Session).WithOne(s => s.Schedule).HasForeignKey<Schedule>(s => s.IdSchedule).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Seat>().HasOne(s => s.Person).WithMany(p => p.Seats).HasForeignKey(s => s.IdPerson).OnDelete(DeleteBehavior.Cascade);

            
        }

    }
}
