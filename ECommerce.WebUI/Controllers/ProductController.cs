using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Interfaces.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // << Bu using'i eklediğinden emin ol!

namespace ECommerce.WebUI.Controllers
{
    // Controller seviyesinde [Authorize] yok, çünkü herkesin ürünleri görmesini istiyoruz.
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ICartService _cartService;
        private readonly IFileStorageService _fileStorageService;

        public ProductController(
            IProductService productService,
            ICategoryRepository categoryRepository,
            IMapper mapper,
            ICartService cartService,
            IFileStorageService fileStorageService)
        {
            _productService = productService;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _cartService = cartService;
            _fileStorageService = fileStorageService;
        }

        // --- PUBLIC (HERKES ERIŞEBILIR) ACTION'LAR ---

        // GET: Product
        // Tüm kullanıcılar (giriş yapmış veya yapmamış) ürünleri listeleyebilir.
        [AllowAnonymous]
        public async Task<IActionResult> Index(ProductFilterAndPaginationDto filter)
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", filter.CategoryId);

            var productListDto = await _productService.GetFilteredAndPaginatedProductsAsync(filter);
            return View(productListDto);
        }

        // GET: Product/Detail/5
        // Tüm kullanıcılar (giriş yapmış veya yapmamış) ürün detaylarını görebilir.
        [AllowAnonymous]
        public async Task<IActionResult> Detail(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var productDto = await _productService.GetProductByIdAsync(id);

            if (productDto == null)
            {
                return NotFound();
            }

            return View(productDto);
        }

        // POST: Product/AddToCart
        // Sepete ekleme işlemi sadece giriş yapmış kullanıcılara açıktır.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Sadece giriş yapmış kullanıcılar sepetine ürün ekleyebilir.
        public async Task<IActionResult> AddToCart(AddToCartDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = string.Join("<br>", ModelState.Values
                                                            .SelectMany(v => v.Errors)
                                                            .Select(e => e.ErrorMessage));
                return RedirectToAction("Detail", new { id = model.ProductId });
            }

            try
            {
                var cartId = _cartService.GetOrCreateCartId();
                await _cartService.AddToCartAsync(cartId, model);

                TempData["SuccessMessage"] = "Ürün sepete eklendi!";
                return RedirectToAction("Detail", new { id = model.ProductId });
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Detail", new { id = model.ProductId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Sepete eklenirken bir hata oluştu: {ex.Message}";
                return RedirectToAction("Detail", new { id = model.ProductId });
            }
        }

        // --- ADMIN (YÖNETICI) ACTION'LAR ---

        // GET: Product/Create
        // Sadece Admin rolündekiler ürün oluşturma formuna erişebilir.
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // POST: Product/Create
        // Sadece Admin rolündekiler ürün oluşturabilir.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateProductDto productDto, IFormFile? mainImageFile, List<IFormFile>? otherImageFiles)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string mainImagePath = null;
                    if (mainImageFile != null && mainImageFile.Length > 0)
                    {
                        using (var stream = mainImageFile.OpenReadStream())
                        {
                            mainImagePath = await _fileStorageService.UploadFileAsync(stream, mainImageFile.FileName, "products");
                        }
                    }

                    var otherImagePaths = new List<string>();
                    if (otherImageFiles != null && otherImageFiles.Any())
                    {
                        var filesToUpload = new List<(Stream fileStream, string fileName)>();
                        foreach (var file in otherImageFiles)
                        {
                            if (file.Length > 0)
                            {
                                filesToUpload.Add((file.OpenReadStream(), file.FileName));
                            }
                        }
                        otherImagePaths = await _fileStorageService.UploadFilesAsync(filesToUpload, "products");
                    }

                    var productImages = new List<ProductImageDto>();
                    if (!string.IsNullOrEmpty(mainImagePath))
                    {
                        productImages.Add(new ProductImageDto { ImagePath = mainImagePath, DisplayOrder = 1, IsMainImage = true });
                    }
                    int order = 2;
                    foreach (var path in otherImagePaths)
                    {
                        productImages.Add(new ProductImageDto { ImagePath = path, DisplayOrder = order++, IsMainImage = false });
                    }
                    productDto.ProductImages = productImages;

                    await _productService.AddNewProductAsync(productDto);
                    TempData["SuccessMessage"] = "Ürün başarıyla eklendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Ürün eklenirken bir hata oluştu: {ex.Message}");
                }
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", productDto.CategoryId);
            return View(productDto);
        }

        // GET: Product/Edit/5
        // Sadece Admin rolündekiler ürün düzenleme formuna erişebilir.
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var productDto = await _productService.GetProductByIdAsync(id); // ProductUpdateDto için ProductImageDto'ları da içermeli
            if (productDto == null)
            {
                return NotFound();
            }

            var updateProductDto = _mapper.Map<UpdateProductDto>(productDto);
            // Mevcut resim yollarını ExistingImagePaths'e aktar
            updateProductDto.ExistingImagePaths = productDto.ProductImages?.Select(pi => pi.ImagePath).ToList() ?? new List<string>();

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", productDto.CategoryId);
            return View(updateProductDto);
        }

        // POST: Product/Edit/5
        // Sadece Admin rolündekiler ürün düzenleyebilir.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, UpdateProductDto productDto, List<IFormFile>? newImageFiles)
        {
            if (id != productDto.Id)
            {
                return NotFound();
            }

            // ModelState kontrolü başta yapılmalı, aksi takdirde kategoriler doldurulmaz
            if (!ModelState.IsValid)
            {
                // Kategorileri tekrar yükle ve view'a dön
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", productDto.CategoryId);
                // Mevcut resim yollarını da tekrar view'a gönder.
                // productDto.ExistingImagePaths zaten formdan geldiği için doğru değeri tutar.
                return View(productDto);
            }

            try
            {
                // Güncel ürünün bilgilerini, özellikle mevcut resimlerini veritabanından çek.
                var existingProductDto = await _productService.GetProductByIdAsync(productDto.Id);
                if (existingProductDto == null)
                {
                    return NotFound();
                }

                // *** RESİM YÖNETİMİ BAŞLANGICI ***

                // Formdan gelen ve tutulması istenen resim yollarını al.
                // View'da checkbox işaretlendiğinde gizli alanın disabled olmaması bu listeyi doğru doldurur.
                var imagesToKeepPathsFromForm = productDto.ExistingImagePaths ?? new List<string>();
                var currentProductImageDtos = new List<ProductImageDto>(); // Servise gönderilecek nihai ProductImageDto listesi.

                // Hangi mevcut resimlerin silineceğini belirle ve fiziksel olarak sil.
                if (existingProductDto.ProductImages != null)
                {
                    foreach (var existingImage in existingProductDto.ProductImages)
                    {
                        // Eğer mevcut resim yolu, formdan gelen tutulacak resimler arasında yoksa, silinmelidir.
                        if (!imagesToKeepPathsFromForm.Contains(existingImage.ImagePath))
                        {
                            _fileStorageService.DeleteFile(existingImage.ImagePath);
                        }
                        else
                        {
                            // Tutulması istenen mevcut resimleri, mevcut bilgileriyle birlikte
                            // nihai listeye ekle. Bu, özellikle Id'leri korur.
                            currentProductImageDtos.Add(existingImage);
                        }
                    }
                }

                // Yeni resimleri yükle ve nihai listeye ekle.
                if (newImageFiles != null && newImageFiles.Any())
                {
                    var filesToUpload = new List<(Stream fileStream, string fileName)>();
                    foreach (var file in newImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            filesToUpload.Add((file.OpenReadStream(), file.FileName));
                        }
                    }
                    var newlyUploadedPaths = await _fileStorageService.UploadFilesAsync(filesToUpload, "products");

                    // Yeni yüklenen resimleri currentProductImageDtos listesine ekle.
                    // DisplayOrder'ı mevcut en yüksek order'dan devam ettir.
                    int newOrder = currentProductImageDtos.Any() ? currentProductImageDtos.Max(pi => pi.DisplayOrder) + 1 : 1;
                    foreach (var path in newlyUploadedPaths)
                    {
                        currentProductImageDtos.Add(new ProductImageDto { ImagePath = path, DisplayOrder = newOrder++, IsMainImage = false });
                    }
                }

                // productDto'nun ProductImages listesini güncellenmiş resimlerle doldur.
                // Bu liste, ProductService'in veritabanı ilişkilerini güncellemesi için kullanılacak.
                productDto.ProductImages = currentProductImageDtos;

                // *** RESİM YÖNETİMİ SONU ***


                // Ürün bilgilerini ve güncellenmiş resim listesini servise gönder.
                await _productService.UpdateProductAsync(productDto);

                TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ürün güncellenirken bir hata oluştu: {ex.Message}");
            }

            // Hata durumunda, kategorileri tekrar yükle ve view'a dön.
            var categoriesOnFailure = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categoriesOnFailure, "Id", "Name", productDto.CategoryId);

            // Eğer view'a geri dönüldüğünde resimler kayboluyorsa, bu satır aktif edilebilir:
            // productDto.ExistingImagePaths = productDto.ProductImages?.Select(pi => pi.ImagePath).ToList() ?? new List<string>();
            return View(productDto);
        }

        // POST: Product/Delete/5
        // Sadece Admin rolündekiler ürün silebilir.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                // Silme işleminden önce ilişkili dosyaları da silmek isterseniz:
                var product = await _productService.GetProductByIdAsync(id); // DTO'yu Entity'e dönüştürdüğünüz Product sınıfı olmalı
                if (product != null && product.ProductImages != null)
                {
                    foreach (var image in product.ProductImages)
                    {
                        _fileStorageService.DeleteFile(image.ImagePath);
                    }
                }

                await _productService.DeleteProductAsync(id);
                TempData["SuccessMessage"] = "Ürün başarıyla silindi.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ürün silinirken bir hata oluştu: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Product/Delete/5 (Opsiyonel: Silmeden önce onay sayfası)
        // Eğer silmeden önce onay sayfası gösterecekseniz bu action'ı kullanın
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}