using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(Guid categoryId);
        Task<Guid> AddNewCategoryAsync(CreateCategoryDto categoryDto);
        Task UpdateCategoryAsync(UpdateCategoryDto categoryDto);
        Task DeleteCategoryAsync(Guid categoryId);
        // Kategoriye özel operasyonlar eklenebilir
    }
}