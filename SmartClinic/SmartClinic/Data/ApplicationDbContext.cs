using Microsoft.EntityFrameworkCore;
using SmartClinic.Models.Entities;
using System.Text.Json;

namespace SmartClinic.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>()
                .HasKey(d => d.DoctorId);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne()
                .HasForeignKey<Doctor>(d => d.UserId);

            // Configure WorkingDays as JSONB
            modelBuilder.Entity<Doctor>()
                .Property(d => d.WorkingDays)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<DayOfWeek>>(v, new JsonSerializerOptions())!
                );
        }
    }
}
