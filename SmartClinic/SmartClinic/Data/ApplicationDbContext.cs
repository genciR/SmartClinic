using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        public DbSet<Appointment> Appointments { get; set; }
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
                      v => JsonSerializer.Deserialize<List<DayOfWeek>>(v, new JsonSerializerOptions())!,
                      new ValueComparer<List<DayOfWeek>>(
                          (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                          c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())) : 0,
                          c => c == null ? new List<DayOfWeek>() : new List<DayOfWeek>(c)
                      )
                  );

            modelBuilder.Entity<Appointment>()
                .HasKey(a => a.AppointmentId);

            // Optional: Add indexes for performance
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.DoctorId, a.StartTime });
        }

    }
}
