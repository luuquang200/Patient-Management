using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PatientManagementApp.Data
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PatientContext>
	{
		public PatientContext CreateDbContext(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			var connectionString = configuration.GetConnectionString("DefaultConnection");

			var optionsBuilder = new DbContextOptionsBuilder<PatientContext>();
			optionsBuilder.UseSqlServer(connectionString);

			return new PatientContext(optionsBuilder.Options);
		}
	}
}
