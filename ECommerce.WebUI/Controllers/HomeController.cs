using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommerce.WebUI.Models; // ErrorViewModel i�in gerekli olabilir
using ECommerce.Application.Interfaces.Services; // IProductService i�in gerekli
using System.Threading.Tasks;
using System.Linq; // LINQ metotlar� i�in gerekli (�imdilik direkt kullan�lmasa da ileride i�e yarayabilir)
using System.Collections.Generic; // List i�in gerekli
using ECommerce.Application.DTOs; // ProductFilterAndPaginationDto ve ProductDto i�in gerekli

namespace ECommerce.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        // Ana ekran i�in Index aksiyonu
        public async Task<IActionResult> Index()
        {
            // ProductFilterAndPaginationDto nesnesi olu�turarak filtreleme ve sayfalama parametrelerini belirliyoruz.
            var filterDto = new ProductFilterAndPaginationDto
            {
                PageNumber = 1,     // �lk sayfa
                PageSize = 8,       // Ana sayfada g�sterilecek �r�n say�s�
                OrderBy = "DateDescending" // En yeni �r�nleri getir
            };

            // IProductService'deki yeni metodu �a��r�yoruz
            var latestProductsListDto = await _productService.GetFilteredAndPaginatedProductsAsync(filterDto);

            // ProductListDto'dan sadece Products listesini View'a g�nderiyoruz.
            // E�er null ise veya Products listesi null ise bo� bir liste g�ndererek View'da hata almay� engelliyoruz.
            return View(latestProductsListDto?.Products ?? new List<ProductDto>());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}