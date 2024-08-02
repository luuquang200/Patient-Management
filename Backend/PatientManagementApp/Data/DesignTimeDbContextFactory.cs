using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PatientManagementApp.Data
{
	public class Shard1ContextFactory : IDesignTimeDbContextFactory<Shard1Context>
    {
        public Shard1Context CreateDbContext(string[] args)
        {
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			var connectionString = configuration.GetConnectionString("SHARD1_MASTER_CONNECTION");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string not found in environment variables.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<Shard1Context>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new Shard1Context(optionsBuilder.Options);
        }
    }

    public class Shard2ContextFactory : IDesignTimeDbContextFactory<Shard2Context>
    {
        public Shard2Context CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();
			
			var connectionString = configuration.GetConnectionString("SHARD2_MASTER_CONNECTION");
			
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string not found in environment variables.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<Shard2Context>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new Shard2Context(optionsBuilder.Options);
        }
    } 
}
