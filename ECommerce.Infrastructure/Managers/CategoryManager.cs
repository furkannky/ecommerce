using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Managers
{
    // ICategoryService arayüzünü implemente eder
    public class CategoryManager : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        // Constructor ile repository ve mapper'ı enjekte ediyoruz
        public CategoryManager(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Guid> AddNewCategoryAsync(CreateCategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto); // DTO'yu Entity'ye dönüştür
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveAsync(); // Değişiklikleri kaydet
            return category.Id;
        }

        public async Task DeleteCategoryAsync(Guid categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                // Hata yönetimi (örneğin NotFoundException fırlatabiliriz)
                throw new Exception("Kategori bulunamadı.");
            }
            _categoryRepository.Delete(category); // Soft delete işaretle
            await _categoryRepository.SaveAsync(); // Değişiklikleri kaydet
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories); // Entity'leri DTO'lara dönüştür
            return categoryDtos;
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(Guid categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return null; // Veya bir özel istisna fırlatabiliriz
            }
            var categoryDto = _mapper.Map<CategoryDto>(category); // Entity'yi DTO'ya dönüştür
            // Eğer ProductCount gibi ek bir bilgi istiyorsak, burada hesaplayabiliriz.
            // categoryDto.ProductCount = category.Products.Count(); // Products ilişkisi yüklüyse
            return categoryDto;
        }

        public async Task UpdateCategoryAsync(UpdateCategoryDto categoryDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(categoryDto.Id);
            if (existingCategory == null)
            {
                throw new Exception("Güncellenecek kategori bulunamadı.");
            }

            // DTO'daki değişiklikleri mevcut entity'ye uygula
            _mapper.Map(categoryDto, existingCategory);
            _categoryRepository.Update(existingCategory); // Entity'yi güncelle (EF Core değişiklikleri izler)
            await _categoryRepository.SaveAsync(); // Değişiklikleri kaydet
        }
    }
}