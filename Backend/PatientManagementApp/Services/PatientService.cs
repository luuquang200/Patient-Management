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
		Task<PatientDto?> GetPatientById(int id);
		Task<PatientDto> AddPatient(CreatePatientDto createPatientDto);
		Task<PatientDto?> UpdatePatient(UpdatePatientDto updatePatientDto);
		Task DeactivatePatient(int id, string reason);
		
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

		public async Task<PatientDto?> GetPatientById(int id)
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
			var patient = await _patientRepository.GetPatientById(updatePatientDto.Id);
			if (patient != null)
			{
				// Check for duplicates
				bool exists = await _patientRepository.PatientExists(updatePatientDto.ContactInfos);
				if (exists)
				{
					throw new InvalidOperationException("A patient with similar contact information already exists.");
				}

				patient.FirstName = updatePatientDto.FirstName;
				patient.LastName = updatePatientDto.LastName;
				patient.Gender = updatePatientDto.Gender;
				patient.DateOfBirth = updatePatientDto.DateOfBirth;
				patient.ContactInfos = updatePatientDto.ContactInfos.Select(c => new ContactInfo
				{
					Type = c.Type,
					Value = c.Value
				}).ToList();
				patient.PrimaryAddress = new Address
				{
					Street = updatePatientDto.PrimaryAddress.Street,
					City = updatePatientDto.PrimaryAddress.City,
					State = updatePatientDto.PrimaryAddress.State,
					ZipCode = updatePatientDto.PrimaryAddress.ZipCode,
					Country = updatePatientDto.PrimaryAddress.Country
				};

				if (updatePatientDto.SecondaryAddress != null)
				{
					patient.SecondaryAddress = new Address
					{
						Street = updatePatientDto.SecondaryAddress.Street,
						City = updatePatientDto.SecondaryAddress.City,
						State = updatePatientDto.SecondaryAddress.State,
						ZipCode = updatePatientDto.SecondaryAddress.ZipCode,
						Country = updatePatientDto.SecondaryAddress.Country
					};
				}
				else
				{
					patient.SecondaryAddress = null;
				}

				await _patientRepository.UpdatePatient(patient);
				return PatientMapper.MapToPatientDto(patient);
			}
			return null;
		}

		public async Task DeactivatePatient(int id, string reason)
		{
			await _patientRepository.DeactivatePatient(id, reason);
		}
	}
}
