// ECommerce.Domain/Entities/AppRole.cs (veya ECommerce.Domain/Identity/AppRole.cs)

using Microsoft.AspNetCore.Identity; // Bu using'i eklemeyi unutmayın

namespace ECommerce.Domain.Entities.Identity // Veya ECommerce.Domain.Identity
{
    public class AppRole : IdentityRole
    {
        // İhtiyaç duyduğunuz ek rol özellikleri buraya gelecek
        // Örnek:
        // public string Description { get; set; }
    }
}