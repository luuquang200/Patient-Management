using Microsoft.EntityFrameworkCore;
using PatientManagementApp.Models;

namespace PatientManagementApp.Data
{
	public class PatientContext : DbContext
	{
		public PatientContext(DbContextOptions<PatientContext> options) : base(options)
		{
		}

		public DbSet<Patient> Patients { get; set; }
		public DbSet<Address> Addresses { get; set; }
		public DbSet<ContactInfo> ContactInfos { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Patient>()
				.HasOne(p => p.PrimaryAddress)
				.WithMany()
				.IsRequired(true)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Patient>()
				.HasOne(p => p.SecondaryAddress)
				.WithMany()
				.IsRequired(false)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Patient>()
				.HasMany(p => p.ContactInfos)
				.WithOne(c => c.Patient)
				.HasForeignKey(c => c.PatientId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
