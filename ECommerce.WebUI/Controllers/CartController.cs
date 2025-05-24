using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ECommerce.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Sepet sayfasını görüntüler
        public async Task<IActionResult> Index()
        {
            var cartId = _cartService.GetOrCreateCartId();
            var cart = await _cartService.GetCartAsync(cartId);
            return View(cart);
        }

        // Sepete ürün ekler (ProductController'dan buraya taşıyabiliriz veya ayrı tutabiliriz)
        // Şimdilik ProductController'da kalsın, ileride buraya taşıyabiliriz.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> AddToCart(AddToCartDto model)
        // {
        //     // ... (ProductController'daki AddToCart metodunun içeriği buraya gelebilir)
        // }

        // Sepetten ürün çıkarır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(Guid productId)
        {
            var cartId = _cartService.GetOrCreateCartId();
            await _cartService.RemoveFromCartAsync(cartId, productId);
            TempData["SuccessMessage"] = "Ürün sepetten çıkarıldı.";
            return RedirectToAction(nameof(Index));
        }

        // Sepetteki ürün miktarını günceller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(Guid productId, int quantity)
        {
            if (quantity <= 0)
            {
                // Miktar 0 veya altına düşerse ürünü çıkar
                return await RemoveFromCart(productId);
            }

            var cartId = _cartService.GetOrCreateCartId();
            try
            {
                await _cartService.UpdateCartItemQuantityAsync(cartId, productId, quantity);
                TempData["SuccessMessage"] = "Ürün miktarı güncellendi.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Miktar güncellenirken bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Sepeti temizler
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            var cartId = _cartService.GetOrCreateCartId();
            await _cartService.ClearCartAsync(cartId);
            TempData["SuccessMessage"] = "Sepetiniz temizlendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}