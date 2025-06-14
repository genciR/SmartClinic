using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SmartClinic.Models.Entities;
using System.Text.Json;

namespace SmartClinic.Data
{
    public class ApplicationDbContext : DbContext
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalHistory> MedicalHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Doctor
            modelBuilder.Entity<Doctor>()
                .HasKey(d => d.DoctorId);
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne()
                .HasForeignKey<Doctor>(d => d.UserId);
            modelBuilder.Entity<Doctor>()
                .Property(d => d.WorkingDays)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<List<DayOfWeek>>(v, _jsonOptions)!,
                    new ValueComparer<List<DayOfWeek>>(
                        (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                        c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())) : 0,
                        c => c == null ? new List<DayOfWeek>() : new List<DayOfWeek>(c)));

            // Patient
            modelBuilder.Entity<Patient>()
                .HasKey(p => p.PatientId);
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<Patient>(p => p.UserId);

            // Appointment
            modelBuilder.Entity<Appointment>()
                .HasKey(a => a.AppointmentId);
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.DoctorId, a.StartTime });

            // MedicalHistory
            modelBuilder.Entity<MedicalHistory>()
                .HasKey(m => m.HistoryId);
            modelBuilder.Entity<MedicalHistory>()
                .HasOne(m => m.Appointment)
                .WithMany()
                .HasForeignKey(m => m.AppointmentId);
            modelBuilder.Entity<MedicalHistory>()
                .HasOne(m => m.Patient)
                .WithMany()
                .HasForeignKey(m => m.PatientId);
            modelBuilder.Entity<MedicalHistory>()
                .HasOne(m => m.Doctor)
                .WithMany()
                .HasForeignKey(m => m.DoctorId);
            modelBuilder.Entity<MedicalHistory>()
                .HasIndex(m => m.PatientId);
        }
    }
}