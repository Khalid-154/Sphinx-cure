using System.ComponentModel.DataAnnotations;

namespace Sphinx_cure_.BLL.ModelVM.Account
{
    public class CreateUserVM
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}
