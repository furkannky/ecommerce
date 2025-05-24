// ECommerce.Domain/Entities/AppUser.cs (veya ECommerce.Domain/Identity/AppUser.cs)
using Microsoft.AspNetCore.Identity; // Bu using'i eklemeyi unutmayın

namespace ECommerce.Domain.Entities.Identity // Veya ECommerce.Domain.Identity
{
    public class AppUser : IdentityUser
    {
        // İhtiyaç duyduğunuz ek kullanıcı özellikleri buraya gelecek
        // Örnek:
        // public string FirstName { get; set; }
        // public string LastName { get; set; }
        // public DateTime BirthDate { get; set; }
    }
}