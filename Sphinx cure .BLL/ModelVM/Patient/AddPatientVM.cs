

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Sphinx_cure_.BLL.ModelVM.Patient
{
    public class AddPatientVM
    {


        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public IFormFile File { get; set; }

        //public string FilePath { get; set; }

    }
}
