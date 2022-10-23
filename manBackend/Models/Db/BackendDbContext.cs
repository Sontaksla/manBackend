using manBackend.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace manBackend.Models
{
    public class BackendDbContext : DbContext
    {
        public BackendDbContext(DbContextOptions<BackendDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOne(i => i.Mail);
            modelBuilder.Entity<User>().HasMany(user => user.Rooms).WithOne();

            modelBuilder.Entity<Classroom>().HasMany(room => room.Students).WithOne();
            modelBuilder.Entity<Classroom>().HasOne(room => room.Teacher).WithMany();
            modelBuilder.Entity<Classroom>().HasMany(room => room.Exercises);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }

    }
}
