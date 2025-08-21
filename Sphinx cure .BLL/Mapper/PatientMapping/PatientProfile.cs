using AutoMapper;
using Sphinx_cure_.BLL.ModelVM.Patient;
using Sphinx_cure_.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphinx_cure_.BLL.Mapper.PatientMapping
{
    // PatientProfile should inherit from AutoMapper.Profile
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientDTO>().ReverseMap();
            CreateMap<AddPatientVM, Patient>();
        }
    }
}
