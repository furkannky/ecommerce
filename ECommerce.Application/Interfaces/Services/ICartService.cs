using ECommerce.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface ICartService
    {
        // Sepete ürün ekler veya mevcut ürünün miktarını günceller
        Task<CartDto> AddToCartAsync(Guid cartId, AddToCartDto addToCartDto);

        // Sepetten ürün çıkarır
        Task<CartDto> RemoveFromCartAsync(Guid cartId, Guid productId);

        // Sepetteki bir ürünün miktarını günceller
        Task<CartDto> UpdateCartItemQuantityAsync(Guid cartId, Guid productId, int quantity);

        // Sepeti getirir (varsa, yoksa boş bir sepet oluşturur)
        Task<CartDto> GetCartAsync(Guid cartId);

        // Sepeti temizler
        Task ClearCartAsync(Guid cartId);

        // Sepet ID'si oluşturur (örneğin yeni bir oturum için)
        Guid GetOrCreateCartId();
    }
}