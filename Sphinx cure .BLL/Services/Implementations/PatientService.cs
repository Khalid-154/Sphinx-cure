using AutoMapper;
using Microsoft.AspNetCore.Http;
using Sphinx_cure_.BLL.Helper;
using Sphinx_cure_.BLL.ModelVM.Patient;
using Sphinx_cure_.BLL.Services.Abstractions;
using Sphinx_cure_.DAL.Entities;
using Sphinx_cure_.DAL.Repo.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphinx_cure_.BLL.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepo _patientRepo;
        private readonly IMapper _mapper;

        public PatientService(IPatientRepo patientRepo, IMapper mapper)
        {
            _patientRepo = patientRepo;
            _mapper = mapper;
        }

        public async Task<(bool status, string message, List<PatientDTO> patients)> GetAllPatientsAsync()
        {
            try
            {
                var patients = await _patientRepo.GetAllAsync();
                var patientDtos = _mapper.Map<List<PatientDTO>>(patients);
                return (true, "Patients retrieved successfully", patientDtos);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", new List<PatientDTO>());
            }
        }

        public async Task<(bool status, string message, PatientDTO? patient)> GetPatientByIdAsync(int id)
        {
            try
            {
                var patient = await _patientRepo.GetByIdAsync(id);
                if (patient == null)
                    return (false, "Patient not found", null);

                var patientDto = _mapper.Map<PatientDTO>(patient);
                return (true, "Patient retrieved successfully", patientDto);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        


        //public async Task<(bool status, string message)> UpdatePatientAsync(PatientDTO patientDto)
        //{
        //    try
        //    {
        //        var patient = _mapper.Map<Patient>(patientDto);
        //        await _patientRepo.UpdateAsync(patient);
        //        await _patientRepo.SaveAsync();
        //        return (true, "Patient updated successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, $"Error: {ex.Message}");
        //    }
        //}

        public async Task<(bool status, string message)> DeletePatientAsync(int id)
        {
            try
            {
                await _patientRepo.DeleteAsync(id);
                await _patientRepo.SaveAsync();
                return (true, "Patient deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool status, string message)> AddPatientAsync(AddPatientVM patientDto, IFormFile file)
        {
            try
            {
                if (patientDto == null)
                    return (false, "Patient data is null");

                if (file == null || file.Length == 0)
                    return (false, "No file uploaded");

                string filePath = Upload.UploadFile("Files", file);

                var patient = _mapper.Map<Patient>(patientDto);
                patient.FilePath = filePath;


                await _patientRepo.AddAsync(patient);
                await _patientRepo.SaveAsync();

                return (true, "Patient added successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error adding patient: {ex.Message}");
            }
        }


        //public async Task<(bool status, string message, List<PatientDTO> patients)> SearchPatientsByNameAsync(string name)
        //{
        //    try
        //    {
        //        var patients = await _patientRepo.SearchByNameAsync(name);
        //        var patientDtos = _mapper.Map<List<PatientDTO>>(patients);
        //        return (true, "Search completed successfully", patientDtos);
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, $"Error: {ex.Message}", new List<PatientDTO>());
        //    }
        //}
    }
}
