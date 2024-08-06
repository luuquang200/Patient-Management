using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.EntityFrameworkCore;
using PatientManagementApp.Data;
using PatientManagementApp.Models;

namespace PatientManagementApp
{
	public static class DataSeeder
	{
		public static async Task SeedDataAsync(Shard1Context shard1Context, Shard2Context shard2Context)
		{
			// Ensure databases are created
			await shard1Context.Database.EnsureCreatedAsync();
			await shard2Context.Database.EnsureCreatedAsync();

			// Generate fake patients
			var patients = GenerateFakePatients(100000);

			await AddPatientsToShards(shard1Context, shard2Context, patients);
		}

		private static List<Patient> GenerateFakePatients(int count)
		{
			var counter = 0;
			var fakePatients = new Faker<Patient>()
				.RuleFor(p => p.PatientId, f => Guid.NewGuid())
				.RuleFor(p => p.FirstName, f => f.Name.FirstName())
				.RuleFor(p => p.LastName, f => f.Name.LastName())
				.RuleFor(p => p.Gender, f => f.PickRandom("Male", "Female"))
				.RuleFor(p => p.DateOfBirth, f => f.Date.Past(80, DateTime.Now.AddYears(-18)))
				.RuleFor(p => p.PrimaryAddress, f => new Address
				{
					Street = f.Address.StreetAddress(),
					City = f.Address.City(),
					State = f.Address.State(),
					ZipCode = f.Address.ZipCode(),
					Country = f.Address.Country()
				})
				.RuleFor(p => p.SecondaryAddress, f => new Address
				{
					Street = f.Address.StreetAddress(),
					City = f.Address.City(),
					State = f.Address.State(),
					ZipCode = f.Address.ZipCode(),
					Country = f.Address.Country()
				})
				.RuleFor(p => p.ContactInfos, (f, p) => new List<ContactInfo>
				{
					new ContactInfo { Type = ContactType.Phone.ToString(), Value = $"{f.Phone.PhoneNumber()}{counter}" },
					new ContactInfo { Type = ContactType.Email.ToString(), Value = $"{p.FirstName.ToLower()}{p.LastName.ToLower()}{counter++}@example.com" }
				})
				.RuleFor(p => p.IsActive, f => true);

			return fakePatients.Generate(count);
		}

		private static async Task AddPatientsToShards(Shard1Context shard1Context, Shard2Context shard2Context, List<Patient> patients)
		{
			var shard1Patients = patients.Where(p => p.PatientId.GetHashCode() % 2 != 0).ToList();
			var shard2Patients = patients.Where(p => p.PatientId.GetHashCode() % 2 == 0).ToList();

			await shard1Context.Patients.AddRangeAsync(shard1Patients);
			await shard1Context.SaveChangesAsync();

			await shard2Context.Patients.AddRangeAsync(shard2Patients);
			await shard2Context.SaveChangesAsync();
		}

		private static DbContext GetMasterContext(Guid patientId, Shard1Context shard1Context, Shard2Context shard2Context)
        {
            var shardId = patientId.GetHashCode() % 2;
            return (shardId == 0) ? shard1Context : shard2Context;
        }

        private static async Task AddPatientsToShard(DbContext context, List<Patient> patients)
        {
            context.Set<Patient>().AddRange(patients);
            await context.SaveChangesAsync();
        }
		public enum ContactType
		{
			Phone,
			Email
		}

		public enum Gender
		{
			Male,
			Female
		}
	}
}
