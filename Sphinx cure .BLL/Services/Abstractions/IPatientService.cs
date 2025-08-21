using Microsoft.AspNetCore.Http;
using Sphinx_cure_.BLL.ModelVM.Patient;
using Sphinx_cure_.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphinx_cure_.BLL.Services.Abstractions
{
    public interface IPatientService
    {
        Task<(bool status, string message, List<PatientDTO> patients)> GetAllPatientsAsync();
        Task<(bool status, string message, PatientDTO? patient)> GetPatientByIdAsync(int id);
        Task<(bool status, string message)> AddPatientAsync(AddPatientVM patientDto,IFormFile file);
        //Task<(bool status, string message)> UpdatePatientAsync(PatientDTO patientDto);
        Task<(bool status, string message)> DeletePatientAsync(int id);
        //Task<(bool status, string message, List<PatientDTO> patients)> SearchPatientsByNameAsync(string name);
    }
}
