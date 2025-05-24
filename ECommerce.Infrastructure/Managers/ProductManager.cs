using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Interfaces.Storage;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore; // Include, CountAsync, ToListAsync için
using System;
using System.Collections.Generic;
using System.Linq; // Where, Skip, Take için
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Managers
{
    public class ProductManager : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository; // Kategori listesi için ihtiyacımız olacak
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public ProductManager(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper, IFileStorageService fileStorageService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        public async Task<Guid> AddNewProductAsync(CreateProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);

            if (productDto.NewImages != null && productDto.NewImages.Any())
            {
                // #### DÜZELTME BAŞLANGICI ####
                var filesToUpload = new List<(System.IO.Stream fileStream, string fileName)>();
                foreach (var file in productDto.NewImages)
                {
                    filesToUpload.Add((file.OpenReadStream(), file.FileName));
                }
                var uploadedImagePaths = await _fileStorageService.UploadFilesAsync(filesToUpload, "products");
                // #### DÜZELTME BİTİŞİ ####

                foreach (var path in uploadedImagePaths)
                {
                    product.ProductImages.Add(new ProductImage { ImagePath = path });
                }
            }

            await _productRepository.AddAsync(product);
            await _productRepository.SaveAsync();
            return product.Id;
        }

        public async Task DeleteProductAsync(Guid productId)
        {
            var product = await _productRepository.GetQuery()
                                                 .Include(p => p.ProductImages)
                                                 .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found.");
            }

            foreach (var image in product.ProductImages)
            {
                _fileStorageService.DeleteFile(image.ImagePath);
            }

            _productRepository.Delete(product);
            await _productRepository.SaveAsync();
        }

        public async Task<ProductDto> GetProductByIdAsync(Guid productId)
        {
            var product = await _productRepository.GetQuery()
                                                 .Include(p => p.Category)
                                                 .Include(p => p.ProductImages)
                                                 .AsNoTracking()
                                                 .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);

            if (product == null)
            {
                return null;
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }

        public async Task UpdateProductAsync(UpdateProductDto productDto)
        {
            var existingProduct = await _productRepository.GetQuery()
                                                            .Include(p => p.ProductImages)
                                                            .FirstOrDefaultAsync(p => p.Id == productDto.Id && !p.IsDeleted);

            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {productDto.Id} not found for update.");
            }

            // AutoMapper ile DTO'dan gelen verileri mevcut ürüne mapleme.
            // Bu işlem existingProduct'ı değiştirir ve EF'nin değişiklik izleyicisini tetikler.
            _mapper.Map(productDto, existingProduct);

            // Silinecek resimleri belirle ve sil
            var existingImagePathsFromDto = productDto.ExistingImagePaths ?? new List<string>();
            var imagesToDelete = existingProduct.ProductImages
                                                .Where(pi => !existingImagePathsFromDto.Contains(pi.ImagePath))
                                                .ToList();

            foreach (var imgToDelete in imagesToDelete)
            {
                _fileStorageService.DeleteFile(imgToDelete.ImagePath); // Dosya sisteminden sil
                existingProduct.ProductImages.Remove(imgToDelete);    // Entity koleksiyonundan kaldır
            }

            // Yeni eklenen resimleri yükle ve ürüne ata
            if (productDto.NewImages != null && productDto.NewImages.Any())
            {
                var newFilesToUpload = new List<(System.IO.Stream fileStream, string fileName)>();
                foreach (var file in productDto.NewImages)
                {
                    newFilesToUpload.Add((file.OpenReadStream(), file.FileName));
                }
                var uploadedNewImagePaths = await _fileStorageService.UploadFilesAsync(newFilesToUpload, "products");

                foreach (var path in uploadedNewImagePaths)
                {
                    existingProduct.ProductImages.Add(new ProductImage { ImagePath = path });
                }
            }

            // #### BURADAKİ SATIRI KALDIRIYORUZ ####
            // _productRepository.Update(existingProduct); // <-- BU SATIRI SİLİN!

            // Değişiklikleri veritabanına kaydet. EF, existingProduct üzerindeki değişiklikleri zaten algılamıştır.
            await _productRepository.SaveAsync();
        }

        // *** BURASI YENİ EKLENEN/GÜNCELLENEN METOD ***
        public async Task<ProductListDto> GetFilteredAndPaginatedProductsAsync(ProductFilterAndPaginationDto filterDto)
        {
            // Temel sorguyu oluştur
            var query = _productRepository.GetQuery()
                                         .Include(p => p.Category) // Kategori bilgisini dahil et
                                         .Include(p => p.ProductImages) // Ürün resimlerini dahil et
                                         .Where(p => !p.IsDeleted); // Sadece silinmemiş ürünleri getir

            // Filtreleme koşullarını uygula
            if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(filterDto.SearchTerm) ||
                                         p.Description.Contains(filterDto.SearchTerm));
            }

            if (filterDto.CategoryId.HasValue && filterDto.CategoryId.Value != Guid.Empty)
            {
                query = query.Where(p => p.CategoryId == filterDto.CategoryId.Value);
            }

            if (filterDto.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filterDto.MinPrice.Value);
            }

            if (filterDto.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filterDto.MaxPrice.Value);
            }

            // Toplam ürün sayısını al (sayfalama öncesi)
            var totalCount = await query.CountAsync();

            // Sayfalama uygula
            var products = await query.OrderBy(p => p.Name) // Ürünleri isme göre sırala (isteğe bağlı ama önerilir)
                                     .Skip((filterDto.PageNumber - 1) * filterDto.PageSize) // Belirli sayıda ürünü atla
                                     .Take(filterDto.PageSize) // Belirli sayıda ürünü al
                                     .ToListAsync(); // Veritabanından veriyi çek

            // Ürünleri DTO'ya maple
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            // ProductListDto nesnesini oluştur ve döndür
            return new ProductListDto
            {
                Products = productDtos,
                PageNumber = filterDto.PageNumber,
                PageSize = filterDto.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / filterDto.PageSize),
                // Filtreleme değerlerini de geri döndürüyoruz ki, UI'da form alanları doldurulabilsin
                SearchTerm = filterDto.SearchTerm,
                CategoryId = filterDto.CategoryId,
                MinPrice = filterDto.MinPrice,
                MaxPrice = filterDto.MaxPrice
            };
        }
    }
}