using Microsoft.EntityFrameworkCore;
using PatientManagementApp.Models;

namespace PatientManagementApp.Data
{
	public class Shard1Context : DbContext
    {
        public Shard1Context(DbContextOptions<Shard1Context> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>()
                .Property(p => p.PatientId)
                .ValueGeneratedNever();
            modelBuilder.Entity<ContactInfo>()
                .HasIndex(c => c.Value)
                .IsUnique();
        }
    }

	public class Shard2Context : DbContext
    {
        public Shard2Context(DbContextOptions<Shard2Context> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>()
                .Property(p => p.PatientId)
                .ValueGeneratedNever();
            modelBuilder.Entity<ContactInfo>()
                .HasIndex(c => c.Value)
                .IsUnique();
        }
    }
}
