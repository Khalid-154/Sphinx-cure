using System.ComponentModel.DataAnnotations;

namespace Sphinx_cure_.BLL.ModelVM.Account
{
    public class CreateUserVM
    {
        [Required]
        [Display(Name = "Username")]
        public required string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        [Display(Name = "Role")]
        public required string Role { get; set; }
    }
}
