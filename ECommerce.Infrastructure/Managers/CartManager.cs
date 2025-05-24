    using ECommerce.Application.DTOs;
    using ECommerce.Application.Interfaces.Repositories; // IProductRepository için
    using ECommerce.Application.Interfaces.Services;
    using Microsoft.AspNetCore.Http; // ISession için
    using System;
    using System.Linq;
    using System.Text.Json; // Sepeti JSON olarak saklamak için
    using System.Threading.Tasks;



    namespace ECommerce.Infrastructure.Managers
    {
        public class CartManager : ICartService
        {
            private const string CartSessionKey = "UserCart"; // Session anahtarı
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IProductRepository _productRepository; // Ürün bilgilerini çekmek için

            public CartManager(IHttpContextAccessor httpContextAccessor, IProductRepository productRepository)
            {
                _httpContextAccessor = httpContextAccessor;
                _productRepository = productRepository;
            }

            // Session'dan sepeti oku veya yeni boş sepet oluştur
            private CartDto GetCurrentCart()
            {
                // Eğer HttpContext veya Session null ise, boş bir sepet dön
                if (_httpContextAccessor.HttpContext?.Session == null)
                {
                    return new CartDto { Id = Guid.NewGuid() }; // Geçici bir ID ile boş sepet
                }

                var cartJson = _httpContextAccessor.HttpContext.Session.GetString(CartSessionKey);
                if (string.IsNullOrEmpty(cartJson))
                {
                    // Yeni bir sepet oluştur ve ona bir ID atayalım
                    var newCart = new CartDto { Id = Guid.NewGuid() };
                    SaveCart(newCart); // Yeni sepeti hemen session'a kaydet
                    return newCart;
                }
                return JsonSerializer.Deserialize<CartDto>(cartJson);
            }

            // Sepeti Session'a kaydet
            private void SaveCart(CartDto cart)
            {
                if (_httpContextAccessor.HttpContext?.Session != null)
                {
                    _httpContextAccessor.HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
                }
            }

            // Sepete ürün ekler veya mevcut ürünün miktarını günceller
            public async Task<CartDto> AddToCartAsync(Guid cartId, AddToCartDto addToCartDto)
            {
                var cart = GetCurrentCart();
                if (cart.Id != cartId) // Güvenlik veya tutarlılık kontrolü, eğer session ID'si değişmişse
                {
                    // Bu senaryoda session ID'si bizim "cartId"miz olduğu için, eğer farklıysa yeni bir sepet oluşturmak daha mantıklı olabilir.
                    // Ancak şimdilik mevcut sepeti kullanmaya devam edelim ve Id'yi güncelleyelim.
                    cart.Id = cartId;
                }

                // Ürünü veritabanından çekerek güncel fiyat ve bilgileri al
                var product = await _productRepository.GetByIdAsync(addToCartDto.ProductId);
                if (product == null || product.IsDeleted)
                {
                    throw new KeyNotFoundException($"Product with ID {addToCartDto.ProductId} not found or is deleted.");
                }

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == addToCartDto.ProductId);

                if (existingItem != null)
                {
                    // Ürün zaten sepetteyse miktarını artır
                    existingItem.Quantity += addToCartDto.Quantity;
                    // Fiyat değişmiş olabileceğinden güncel fiyatı tekrar ata
                    existingItem.UnitPrice = product.Price;
                }
                else
                {
                    // Ürün sepette yoksa yeni bir sepet öğesi oluştur
                    cart.Items.Add(new CartItemDto
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        Quantity = addToCartDto.Quantity,
                        ProductImage = product.ProductImages.FirstOrDefault()?.ImagePath // İlk resmi al
                    });
                }

                SaveCart(cart);
                return cart;
            }

            // Sepetten ürün çıkarır
            public async Task<CartDto> RemoveFromCartAsync(Guid cartId, Guid productId)
            {
                var cart = GetCurrentCart();
                if (cart.Id != cartId) return cart; // Güvenlik veya tutarlılık kontrolü

                var itemToRemove = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (itemToRemove != null)
                {
                    cart.Items.Remove(itemToRemove);
                    SaveCart(cart);
                }
                return cart;
            }

            // Sepetteki bir ürünün miktarını günceller
            public async Task<CartDto> UpdateCartItemQuantityAsync(Guid cartId, Guid productId, int quantity)
            {
                if (quantity <= 0)
                {
                    return await RemoveFromCartAsync(cartId, productId); // Miktar 0 veya altına inerse ürünü kaldır
                }

                var cart = GetCurrentCart();
                if (cart.Id != cartId) return cart; // Güvenlik veya tutarlılık kontrolü

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

                if (existingItem != null)
                {
                    // Ürünün güncel fiyatını veritabanından çekerek tutarlılığı sağla
                    var product = await _productRepository.GetByIdAsync(productId);
                    if (product == null || product.IsDeleted)
                    {
                        // Ürün bulunamazsa veya silinmişse sepetten kaldır
                        await RemoveFromCartAsync(cartId, productId);
                        throw new KeyNotFoundException($"Product with ID {productId} not found or is deleted. Item removed from cart.");
                    }

                    existingItem.Quantity = quantity;
                    existingItem.UnitPrice = product.Price; // Fiyat güncelliğini koru
                    SaveCart(cart);
                }
                return cart;
            }

            // Sepeti getirir
            public async Task<CartDto> GetCartAsync(Guid cartId)
            {
                var cart = GetCurrentCart();
                // GetCartAsync metoduna dışarıdan bir cartId verildiğinde, session'daki ID ile uyumlu mu kontrol et
                if (cart.Id == Guid.Empty || cart.Id != cartId)
                {
                    // Eğer cartId uyumlu değilse veya session'da cart ID'si yoksa
                    // Bu, yeni bir oturum veya farklı bir kullanıcı için yeni bir sepet anlamına gelebilir.
                    // Basitlik adına, burada session'daki sepeti döndürüyoruz.
                    // Daha kompleks senaryolarda, cartId'ye göre veritabanından sepet çekmek gerekebilir.
                    // Şimdilik session'daki ID'yi dışarıdan gelenle eşitliyoruz, bu biraz basitleştirme.
                    cart.Id = cartId;
                    SaveCart(cart); // ID'yi güncellediğimiz için kaydet
                }

                // Her çağrıda ürün fiyatlarının güncelliğini kontrol edelim
                foreach (var item in cart.Items.ToList()) // ToList() ile döngü sırasında koleksiyonun değişmesini engelle
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null || product.IsDeleted)
                    {
                        // Ürün artık yoksa sepetten kaldır
                        cart.Items.Remove(item);
                    }
                    else if (item.UnitPrice != product.Price)
                    {
                        // Fiyat değişmişse güncelle
                        item.UnitPrice = product.Price;
                    }
                }
                SaveCart(cart); // Güncel sepeti kaydet
                return cart;
            }


            // Sepeti temizler
            public Task ClearCartAsync(Guid cartId)
            {
                if (_httpContextAccessor.HttpContext?.Session != null)
                {
                    _httpContextAccessor.HttpContext.Session.Remove(CartSessionKey);
                }
                return Task.CompletedTask; // Asenkron imzaya uymak için
            }

            // Sepet ID'si oluşturur (genellikle ilk istekte kullanılır)
            public Guid GetOrCreateCartId()
            {
                var cart = GetCurrentCart();
                return cart.Id;
            }
        }
    }