// ECommerce.WebUI/Controllers/CategoryController.cs

using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace ECommerce.WebUI.Controllers
{
    // Controller seviyesinde [Authorize] yok, çünkü herkesin kategorileri görmesini istiyoruz.
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        // --- PUBLIC (HERKES ERİŞEBİLİR) ACTION'LAR ---

        // GET: Category
        // Tüm kullanıcılar (giriş yapmış veya yapmamış) kategorileri listeleyebilir.
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        // --- ADMIN (YÖNETİCİ) ACTION'LAR ---

        // Kategori ekleme formu (GET)
        [HttpGet]
        [Authorize(Roles = "Admin")] // Sadece Admin rolündekiler erişebilir
        public IActionResult Create()
        {
            return View();
        }

        // Kategori ekleme işlemi (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // Sadece Admin rolündekiler ekleyebilir
        public async Task<IActionResult> Create(CreateCategoryDto model)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.AddNewCategoryAsync(model);
                TempData["SuccessMessage"] = "Kategori başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // Kategori düzenleme formu (GET)
        [HttpGet]
        [Authorize(Roles = "Admin")] // Sadece Admin rolündekiler erişebilir
        public async Task<IActionResult> Edit(Guid id)
        {
            // _categoryService.GetCategoryByIdAsync(id) artık CategoryDto döndürüyor.
            var categoryDto = await _categoryService.GetCategoryByIdAsync(id); // Değişken adını categoryDto yaptık

            if (categoryDto == null)
            {
                return NotFound();
            }

            // AutoMapper, CategoryDto'dan UpdateCategoryDto'ya dönüşümü yapacak.
            // GeneralMapping'de CreateMap<CategoryDto, UpdateCategoryDto>(); kuralı eklendi.
            var updateModel = _mapper.Map<UpdateCategoryDto>(categoryDto);
            return View(updateModel);
        }

        // Kategori düzenleme işlemi (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // Sadece Admin rolündekiler düzenleyebilir
        public async Task<IActionResult> Edit(UpdateCategoryDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _categoryService.UpdateCategoryAsync(model);
                    TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Kategori güncellenirken bir hata oluştu: {ex.Message}");
                }
            }
            return View(model);
        }

        // Kategori silme işlemi (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // Sadece Admin rolündekiler silebilir
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Kategori silinirken bir hata oluştu: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Delete/5 (Opsiyonel: Silmeden önce onay sayfası)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
    }
}