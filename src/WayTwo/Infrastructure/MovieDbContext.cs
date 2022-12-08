using Microsoft.EntityFrameworkCore;
using WayTwo.Models;

namespace WayTwo.Infrastructure
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Movie>()
                .HasIndex(x => x.Title);

            modelBuilder
                .Entity<Movie>()
                .HasData(GetMovies());
        }

        private static Movie[] GetMovies()
        {
            return new[]
            {
                new Movie
                {
                    Id = 1,
                    Title = "Titanic",
                    ReleaseDate = new DateTime(1998, 01, 15)
                },
                new Movie
                {
                    Id = 2,
                    Title = "The Sixth Sense",
                    ReleaseDate = new DateTime(1999, 12, 23)
                }
            };
        }
    }
}
