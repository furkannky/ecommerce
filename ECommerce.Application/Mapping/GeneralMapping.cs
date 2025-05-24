// ECommerce.Application/Mapping/GeneralMapping.cs

using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            // Product Mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ReverseMap();
            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.ProductImages, opt => opt.Ignore());
            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.ProductImages, opt => opt.Ignore());

            // ProductDto'dan UpdateProductDto'ya dönüşüm (Ürünler için, eğer kullanılıyorsa)
            CreateMap<ProductDto, UpdateProductDto>()
                .ForMember(dest => dest.NewImages, opt => opt.Ignore())
                .ForMember(dest => dest.ExistingImagePaths, opt => opt.Ignore());

            // Category Mappings
            CreateMap<Category, CategoryDto>().ReverseMap(); // Category Entity <-> Category DTO
            CreateMap<CreateCategoryDto, Category>();       // CreateCategory DTO -> Category Entity
            CreateMap<UpdateCategoryDto, Category>();       // UpdateCategory DTO -> Category Entity

            // <<< BU SATIR EKLENDİ VE KATEGORİ DÜZENLEME HATASINI ÇÖZMELİDİR! >>>
            CreateMap<CategoryDto, UpdateCategoryDto>();    // Category DTO -> UpdateCategory DTO (Hata bu yüzden oluyordu)
            // <<< YUKARIDAKİ SATIR EKLENDİ >>>

            // ProductImage Mappings
            CreateMap<ProductImage, ProductImageDto>().ReverseMap();
        }
    }
}