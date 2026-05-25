using Microsoft.AspNetCore.Identity;

namespace LoginMVC.Identity;

public class ApplicationUser : IdentityUser
{
    public string Fullname { get; set; } = default!;
}
