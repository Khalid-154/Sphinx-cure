using System.ComponentModel.DataAnnotations;
namespace Sphinx_cure_.BLL.ModelVM.Account;
public class LoginVM
{
    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}