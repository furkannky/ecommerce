using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommerce.WebUI.Models; // ErrorViewModel için gerekli olabilir
using ECommerce.Application.Interfaces.Services; // IProductService için gerekli
using System.Threading.Tasks;
using System.Linq; // LINQ metotlarý için gerekli (þimdilik direkt kullanýlmasa da ileride iþe yarayabilir)
using System.Collections.Generic; // List için gerekli
using ECommerce.Application.DTOs; // ProductFilterAndPaginationDto ve ProductDto için gerekli

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

        // Ana ekran için Index aksiyonu
        public async Task<IActionResult> Index()
        {
            // ProductFilterAndPaginationDto nesnesi oluþturarak filtreleme ve sayfalama parametrelerini belirliyoruz.
            var filterDto = new ProductFilterAndPaginationDto
            {
                PageNumber = 1,     // Ýlk sayfa
                PageSize = 8,       // Ana sayfada gösterilecek ürün sayýsý
                OrderBy = "DateDescending" // En yeni ürünleri getir
            };

            // IProductService'deki yeni metodu çaðýrýyoruz
            var latestProductsListDto = await _productService.GetFilteredAndPaginatedProductsAsync(filterDto);

            // ProductListDto'dan sadece Products listesini View'a gönderiyoruz.
            // Eðer null ise veya Products listesi null ise boþ bir liste göndererek View'da hata almayý engelliyoruz.
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