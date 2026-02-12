using Microsoft.EntityFrameworkCore;
using SMS.Models;

namespace SMS.DAL
{
    public class SMSDbContext : DbContext
    {
        public SMSDbContext(DbContextOptions<SMSDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Qualification>()
                .HasOne(q => q.Student)
                .WithMany(s => s.Qualifications)
                .HasForeignKey(q => q.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed initial data if needed, or configure indexes
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Username)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();
        }
    }
}
