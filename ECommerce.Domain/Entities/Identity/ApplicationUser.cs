// ECommerce.Domain/Entities/Identity/ApplicationUser.cs
using Microsoft.AspNetCore.Identity; // IdentityUser için bu using gerekli

namespace ECommerce.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        // Ekstra kullanıcı özellikleri buraya eklenebilir, şu anki haliyle boş kalabilir.
    }
}