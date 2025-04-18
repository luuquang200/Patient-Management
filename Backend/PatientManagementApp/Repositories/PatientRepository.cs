using Microsoft.EntityFrameworkCore;
using PatientManagementApp.Data;
using PatientManagementApp.DTOs;
using PatientManagementApp.Helpers;
using PatientManagementApp.Models;
using PatientManagementApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientManagementApp.Repositories
{
	public interface IPatientRepository
	{
		Task<IEnumerable<Patient>> GetPatients();
		Task<PaginatedList<Patient>> SearchPatients(string? searchTerm, int page, int pageSize);
		Task<Patient> GetPatientById(Guid id);
		Task AddPatient(Patient patient);
		Task<Patient?> UpdatePatient(UpdatePatientDto updatePatientDto);
		Task DeactivatePatient(Guid id, string reason);
		Task<bool> PatientExists(List<ContactInfoDto> contactInfos);
		Task<bool> PatientExists(List<ContactInfoDto> contactInfos, Guid excludePatientId);
	}

	public class PatientRepository : IPatientRepository
	{
		private readonly Shard1Context _shard1ContextMaster;
		private readonly Shard2Context _shard2ContextMaster;
		private readonly Shard1Context _shard1ContextReplica;
		private readonly Shard2Context _shard2ContextReplica;
		private readonly IGeneratorIdService _generatorIdService;

		public PatientRepository(
			Shard1Context shard1ContextMaster,
			Shard2Context shard2ContextMaster,
			Shard1Context shard1ContextReplica,
			Shard2Context shard2ContextReplica,
			IGeneratorIdService generatorIdService)
		{
			_shard1ContextMaster = shard1ContextMaster;
			_shard2ContextMaster = shard2ContextMaster;
			_shard1ContextReplica = shard1ContextReplica;
			_shard2ContextReplica = shard2ContextReplica;
			_generatorIdService = generatorIdService;
		}

		private DbContext GetMasterContext(Guid patientId)
		{
			var shardId = patientId.GetHashCode() % 2;
			return (shardId == 0) ? _shard2ContextMaster : _shard1ContextMaster;
		}

		private DbContext GetReplicaContext(Guid patientId)
		{
			var shardId = patientId.GetHashCode() % 2;
			return (shardId == 0) ? _shard2ContextReplica : _shard1ContextReplica;
		}

		public async Task<IEnumerable<Patient>> GetPatients()
		{
			var patientsShard1 = _shard1ContextReplica.Patients.Include(p => p.ContactInfos)
				.Include(p => p.PrimaryAddress)
				.Include(p => p.SecondaryAddress)
				.ToListAsync();

			var patientsShard2 = _shard2ContextReplica.Patients.Include(p => p.ContactInfos)
				.Include(p => p.PrimaryAddress)
				.Include(p => p.SecondaryAddress)
				.ToListAsync();

			await Task.WhenAll(patientsShard1, patientsShard2);
			
			return patientsShard1.Result.Concat(patientsShard2.Result);
		}

		public async Task<PaginatedList<Patient>> SearchPatients(string? searchTerm, int page, int pageSize)
		{
			IQueryable<Patient> query1 = _shard1ContextReplica.Patients.AsNoTracking().Include(p => p.ContactInfos)
				.Include(p => p.PrimaryAddress)
				.Include(p => p.SecondaryAddress);

			IQueryable<Patient> query2 = _shard2ContextReplica.Patients.AsNoTracking().Include(p => p.ContactInfos)
				.Include(p => p.PrimaryAddress)
				.Include(p => p.SecondaryAddress);

			if (!string.IsNullOrEmpty(searchTerm))
			{
				var searchTerms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

				foreach (var term in searchTerms)
				{
					query1 = query1.Where(p => p.FirstName.Contains(term) ||
											p.LastName.Contains(term) ||
											p.DateOfBirth.Date.ToString().Contains(term) ||
											p.ContactInfos.Any(c => c.Value.Contains(term)));

					query2 = query2.Where(p => p.FirstName.Contains(term) ||
											p.LastName.Contains(term) ||
											p.DateOfBirth.Date.ToString().Contains(term) ||
											p.ContactInfos.Any(c => c.Value.Contains(term)));
				}
			}

			var results1Task = query1.ToListAsync();
			var results2Task = query2.ToListAsync();

			await Task.WhenAll(results1Task, results2Task);

			var results1 = results1Task.Result;
			var results2 = results2Task.Result;

			var combinedResults = results1.Concat(results2).ToList();

			var count = combinedResults.Count();
			var items = combinedResults.Skip((page - 1) * pageSize)
									.Take(pageSize)
									.ToList();

			return new PaginatedList<Patient>(items, count, page, pageSize);
		}

		public async Task<Patient> GetPatientById(Guid id)
		{
			var context = GetReplicaContext(id);
			return await context.Set<Patient>().AsNoTracking().Include(p => p.ContactInfos)
				.Include(p => p.PrimaryAddress)
				.Include(p => p.SecondaryAddress)
				.FirstOrDefaultAsync(p => p.PatientId == id);
		}

		public async Task AddPatient(Patient patient)
		{
			patient.PatientId = _generatorIdService.GenerateId();
			var context = GetMasterContext(patient.PatientId);
			context.Set<Patient>().Add(patient);
			await context.SaveChangesAsync();
		}

		public async Task<Patient?> UpdatePatient(UpdatePatientDto updatePatientDto)
		{
			var context = GetMasterContext(updatePatientDto.Id);

			var patient = await context.Set<Patient>()
				.Include(p => p.ContactInfos)
				.Include(p => p.PrimaryAddress)
				.Include(p => p.SecondaryAddress)
				.FirstOrDefaultAsync(p => p.PatientId == updatePatientDto.Id);

			if (patient == null)
			{
				return null;
			}

			patient.FirstName = updatePatientDto.FirstName;
			patient.LastName = updatePatientDto.LastName;
			patient.Gender = updatePatientDto.Gender;
			patient.DateOfBirth = updatePatientDto.DateOfBirth;

			// Update the primary address
			if (patient.PrimaryAddress != null)
			{
				patient.PrimaryAddress.Street = updatePatientDto.PrimaryAddress.Street;
				patient.PrimaryAddress.City = updatePatientDto.PrimaryAddress.City;
				patient.PrimaryAddress.State = updatePatientDto.PrimaryAddress.State;
				patient.PrimaryAddress.ZipCode = updatePatientDto.PrimaryAddress.ZipCode;
				patient.PrimaryAddress.Country = updatePatientDto.PrimaryAddress.Country;
			}

			// Update the secondary address
			if (IsAddressNullOrEmpty(updatePatientDto.SecondaryAddress))
			{
				// If the secondary address is empty, remove it
				patient.SecondaryAddress = null;
			}
			else
			{
				if (patient.SecondaryAddress == null)
				{
					patient.SecondaryAddress = new Address();
				}
				patient.SecondaryAddress.Street = updatePatientDto.SecondaryAddress.Street;
				patient.SecondaryAddress.City = updatePatientDto.SecondaryAddress.City;
				patient.SecondaryAddress.State = updatePatientDto.SecondaryAddress.State;
				patient.SecondaryAddress.ZipCode = updatePatientDto.SecondaryAddress.ZipCode;
				patient.SecondaryAddress.Country = updatePatientDto.SecondaryAddress.Country;
			}

			// Remove old contact infos
			context.Set<ContactInfo>().RemoveRange(patient.ContactInfos);

			patient.ContactInfos = updatePatientDto.ContactInfos.Select(c => new ContactInfo
			{
				Type = c.Type,
				Value = c.Value,
				PatientId = patient.PatientId
			}).ToList();

			await context.SaveChangesAsync();
			return patient;
		}

		private bool IsAddressNullOrEmpty(AddressDto? address)
		{
			return address == null ||
				   string.IsNullOrWhiteSpace(address.Street) &&
				   string.IsNullOrWhiteSpace(address.City) &&
				   string.IsNullOrWhiteSpace(address.State) &&
				   string.IsNullOrWhiteSpace(address.ZipCode) &&
				   string.IsNullOrWhiteSpace(address.Country);
		}

		public async Task DeactivatePatient(Guid id, string reason)
		{
			var context = GetMasterContext(id);
			var patient = await context.Set<Patient>().FindAsync(id);
			if (patient != null)
			{
				patient.IsActive = false;
				patient.InactiveReason = reason;
				context.Set<Patient>().Update(patient);
				await context.SaveChangesAsync();
			}
		}

		public async Task<bool> PatientExists(List<ContactInfoDto> contactInfos)
		{
			var contactValues = contactInfos.Select(c => c.Value).ToList();

			var shard1Task = _shard1ContextReplica.ContactInfos.AnyAsync(c => contactValues.Contains(c.Value));
			var shard2Task = _shard2ContextReplica.ContactInfos.AnyAsync(c => contactValues.Contains(c.Value));

			var results = await Task.WhenAll(shard1Task, shard2Task);

			return results.Any(result => result);
		}

		public async Task<bool> PatientExists(List<ContactInfoDto> contactInfos, Guid excludePatientId)
		{
			var contactValues = contactInfos.Select(c => c.Value).ToList();

			foreach (var contactInfo in _shard1ContextReplica.ContactInfos)
			{
				if (contactValues.Contains(contactInfo.Value) && contactInfo.PatientId != excludePatientId)
				{
					return true;
				}
			}

			var shard1Task = _shard1ContextReplica.ContactInfos.AnyAsync(c => contactValues.Contains(c.Value) && c.PatientId != excludePatientId);
			var shard2Task = _shard2ContextReplica.ContactInfos.AnyAsync(c => contactValues.Contains(c.Value) && c.PatientId != excludePatientId);

			var results = await Task.WhenAll(shard1Task, shard2Task);

			return results.Any(result => result);
		}
	}
}
