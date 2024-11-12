using Microsoft.EntityFrameworkCore;
using TaskFlow.Models;


namespace TaskFlow.Services
{
    public class DatabaseService : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Host=202.10.42.56;Port=5432;Database=aplikasi_taskflow;Username=postgres;Password=projects";
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql("uuid_generate_v4()");

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Id)
                .HasDefaultValueSql("uuid_generate_v4()");

            base.OnModelCreating(modelBuilder);
        }
    }
}
