using PatientManagementApp.DTOs;
using PatientManagementApp.Helpers;
using PatientManagementApp.Models;
using PatientManagementApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientManagementApp.Services
{
	public interface IPatientService
	{
		Task<IEnumerable<PatientDto>> GetPatients();
		Task<PaginatedList<PatientDto>> SearchPatients(string? searchTerm, int page, int pageSize);
		Task<PatientDto?> GetPatientById(Guid id);
		Task<PatientDto> AddPatient(CreatePatientDto createPatientDto);
		Task<PatientDto?> UpdatePatient(UpdatePatientDto updatePatientDto);
		Task DeactivatePatient(Guid id, string reason);
		
	}
	public class PatientService : IPatientService
	{
		private readonly IPatientRepository _patientRepository;

		public PatientService(IPatientRepository patientRepository)
		{
			_patientRepository = patientRepository;
		}

		public async Task<IEnumerable<PatientDto>> GetPatients()
		{
			var patients = await _patientRepository.GetPatients();
			return patients.Select(p => PatientMapper.MapToPatientDto(p));
		}

		public async Task<PaginatedList<PatientDto>> SearchPatients(string? searchTerm, int page, int pageSize)
		{
			var paginatedPatients = await _patientRepository.SearchPatients(searchTerm, page, pageSize);
			var patientDtos = paginatedPatients.Items.Select(p => PatientMapper.MapToPatientDto(p)).ToList();

			return new PaginatedList<PatientDto>(patientDtos, paginatedPatients.TotalCount, paginatedPatients.PageIndex, pageSize);
		}

		public async Task<PatientDto?> GetPatientById(Guid id)
		{
			var patient = await _patientRepository.GetPatientById(id);
			if (patient != null)
			{
				return PatientMapper.MapToPatientDto(patient);
			}
			return null;
		}

		public async Task<PatientDto> AddPatient(CreatePatientDto createPatientDto)
		{
			// Check for duplicates
			bool exists = await _patientRepository.PatientExists(createPatientDto.ContactInfos);
			if (exists)
			{
				throw new InvalidOperationException("A patient with similar contact information already exists.");
			}

			var patient = PatientMapper.MapToEntity(createPatientDto);
			await _patientRepository.AddPatient(patient);
			return PatientMapper.MapToPatientDto(patient);
		}

		public async Task<PatientDto?> UpdatePatient(UpdatePatientDto updatePatientDto)
		{
			// Check for duplicates
			bool exists = await _patientRepository.PatientExists(updatePatientDto.ContactInfos, updatePatientDto.Id);
			if (exists)
			{
				throw new InvalidOperationException("A patient with similar contact information already exists.");
			}

			var updatedPatient = await _patientRepository.UpdatePatient(updatePatientDto);
			return updatedPatient != null ? PatientMapper.MapToPatientDto(updatedPatient) : null;
		}



		public async Task DeactivatePatient(Guid id, string reason)
		{
			await _patientRepository.DeactivatePatient(id, reason);
		}
	}
}
