using System.ComponentModel.DataAnnotations;
namespace Sphinx_cure_.BLL.ModelVM.Account;

public class LoginVM
{
    [Required]
    [Display(Name = "Username")]
    public required string UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    public bool RememberMe { get; set; }
}