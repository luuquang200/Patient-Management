using Microsoft.AspNetCore.Mvc;
using PatientManagementApp.DTOs;
using PatientManagementApp.Helpers;
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
        public async Task<ActionResult<ApiResponse<PatientDto>>> CreatePatient([FromBody] CreatePatientDto createPatientDto)
        {
            var response = new ApiResponse<PatientDto>();

            try
            {
                var patientDto = await _patientService.AddPatient(createPatientDto);
                response.Data = patientDto;
                response.Message = "Patient created successfully";
                return CreatedAtAction("GetPatient", new { id = patientDto.Id }, response);
            }
            catch (Exception ex)
            {
                // Log error
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        // GET: api/patients/search
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<PaginatedList<PatientDto>>>> SearchPatients([FromQuery] string? searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var response = new ApiResponse<PaginatedList<PatientDto>>();

            try
            {
                var paginatedPatients = await _patientService.SearchPatients(searchTerm, page, pageSize);
                response.Data = paginatedPatients;
                response.Message = "Patients retrieved successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log error
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        // GET: api/patients/get/{id}
        [HttpGet("get/{id}")]
        public async Task<ActionResult<ApiResponse<PatientDto>>> GetPatient(int id)
        {
            var response = new ApiResponse<PatientDto>();

            try
            {
                var patient = await _patientService.GetPatientById(id);
                if (patient == null)
                {
                    response.Success = false;
                    response.Message = "Patient not found";
                    return NotFound(response);
                }
                response.Data = patient;
                response.Message = "Patient retrieved successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log error
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        // PUT: api/patients/update
        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse<PatientDto>>> UpdatePatient([FromBody] UpdatePatientDto updatePatientDto)
        {
            var response = new ApiResponse<PatientDto>();

            try
            {
                var patientDto = await _patientService.UpdatePatient(updatePatientDto);
                if (patientDto == null)
                {
                    response.Success = false;
                    response.Message = "Patient not found";
                    return NotFound(response);
                }
                response.Message = "Patient updated successfully";
				response.Data = patientDto;
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log error
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        // DELETE: api/patients/deactivate/{id}
        [HttpDelete("deactivate/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeactivatePatient(int id, [FromBody] string reason)
        {
            var response = new ApiResponse<string>();

            try
            {
                var patient = await _patientService.GetPatientById(id);
                if (patient == null)
                {
                    response.Success = false;
                    response.Message = "Patient not found";
                    return NotFound(response);
                }

                await _patientService.DeactivatePatient(id, reason);
                response.Message = "Patient deactivated successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log error
                response.Success = false;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }
    }
}
