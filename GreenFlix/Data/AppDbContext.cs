using GreenFlix.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GreenFlix.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<WatchedMovie> WatchedMovies { get; set; }
        public DbSet<FavoriteMovie> FavoriteMovies { get; set; }
        public DbSet<MovieComment> MovieComments { get; set; }

    }
}



