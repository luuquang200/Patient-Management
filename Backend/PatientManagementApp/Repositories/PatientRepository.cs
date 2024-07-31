using Microsoft.EntityFrameworkCore;
using PatientManagementApp.Data;
using PatientManagementApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientManagementApp.Repositories
{
	public interface IPatientRepository
	{
		Task<IEnumerable<Patient>> GetPatients();
		Task<Patient> GetPatientById(int id);
		Task AddPatient(Patient patient);
		Task UpdatePatient(Patient patient);
		Task DeactivatePatient(int id, string reason);
		Task<bool> PatientExists(int id);
	}
	public class PatientRepository : IPatientRepository
	{
		private readonly PatientContext _context;

		public PatientRepository(PatientContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Patient>> GetPatients()
		{
			return await _context.Patients.Include(p => p.ContactInfos)
										  .Include(p => p.PrimaryAddress)
										  .Include(p => p.SecondaryAddress)
										  .ToListAsync();
		}

		public async Task<Patient> GetPatientById(int id)
		{
			return await _context.Patients.Include(p => p.ContactInfos)
										  .Include(p => p.PrimaryAddress)
										  .Include(p => p.SecondaryAddress)
										  .FirstOrDefaultAsync(p => p.PatientId == id);
		}

		public async Task AddPatient(Patient patient)
		{
			_context.Patients.Add(patient);
			await _context.SaveChangesAsync();
		}

		public async Task UpdatePatient(Patient patient)
		{
			_context.Entry(patient).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task DeactivatePatient(int id, string reason)
		{
			var patient = await _context.Patients.FindAsync(id);
			if (patient != null)
			{
				patient.IsActive = false;
				patient.InactiveReason = reason;
				_context.Entry(patient).State = EntityState.Modified;
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> PatientExists(int id)
		{
			return await _context.Patients.AnyAsync(e => e.PatientId == id);
		}
	}
}
