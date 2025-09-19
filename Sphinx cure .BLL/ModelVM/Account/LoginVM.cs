

using System.ComponentModel.DataAnnotations;

namespace Sphinx_cure_.BLL.ModelVM.Account
{
    public class LoginVM
    {
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Email { get; set; }

    }
}
