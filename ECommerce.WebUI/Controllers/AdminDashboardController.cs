using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebUI.Controllers
{
    [Authorize(Roles = "Admin")] // Bu controller'a sadece Admin rolündekiler erişebilir
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Admin Paneli";
            return View();
        }

        // Buraya admin'e özel diğer yönetim action'ları gelebilir
        // Örneğin: Kullanıcıları Yönet, Siparişleri Gör, Ürünleri Yönet (ProductController'daki admin action'larını buraya taşıyabilirsin)
    }
}