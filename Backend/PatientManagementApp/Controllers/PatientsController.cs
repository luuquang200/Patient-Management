using Microsoft.AspNetCore.Mvc;
using PatientManagementApp.DTOs;
using PatientManagementApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatientManagementApp.Controllers
{
	[Route("api/patients")]
	[ApiController]
	public class PatientsController : ControllerBase
	{
		private readonly IPatientService _patientService;

		public PatientsController(IPatientService patientService)
		{
			_patientService = patientService;
		}

		// POST: api/patients/create
		[HttpPost("create")]
		public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] CreatePatientDto createPatientDto)
		{
			try
			{
				PatientDto patientDto = await _patientService.AddPatient(createPatientDto);
				return CreatedAtAction("GetPatient", new { id = patientDto.Id }, patientDto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}

		// GET: api/patients/search
		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
		{
			try
			{
				var patients = await _patientService.GetPatients();
				return Ok(patients);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}

		// GET: api/patients/get/{id}
		[HttpGet("get/{id}")]
		public async Task<ActionResult<PatientDto>> GetPatient(int id)
		{
			try
			{
				var patient = await _patientService.GetPatientById(id);
				if (patient == null)
				{
					return NotFound();
				}
				return Ok(patient);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}

		// PUT: api/patients/update/{id}
		[HttpPut("update/{id}")]
		public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientDto updatePatientDto)
		{
			try
			{
				await _patientService.UpdatePatient(updatePatientDto);
				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}

		// DELETE: api/patients/deactivate/{id}
		[HttpDelete("deactivate/{id}")]
		public async Task<IActionResult> DeactivatePatient(int id, [FromBody] string reason)
		{
			try
			{
				var patient = await _patientService.GetPatientById(id);
				if (patient == null)
				{
					return NotFound();
				}

				await _patientService.DeactivatePatient(id, reason);
				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}
	}
}
