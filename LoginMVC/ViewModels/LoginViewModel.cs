using System.ComponentModel.DataAnnotations;

namespace LoginMVC.ViewModels;

public class LoginViewModel
{
    [EmailAddress]
    [Required]
    public string? Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}
