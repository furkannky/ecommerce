using ECommerce.Domain.Common;
using System.Linq.Expressions; // Expression'lar için

namespace ECommerce.Application.Interfaces.Repositories
{
    // T, BaseEntity'den miras alan bir sınıf olmalı
    public interface IRepository<T> where T : BaseEntity
    {
        Task<List<T>> GetAllAsync(); // Tüm verileri getir
        Task<T> GetByIdAsync(Guid id); // ID'ye göre tek veri getir
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate); // Belirli bir koşula göre tek veri getir
        Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> predicate); // Belirli bir koşula göre tüm verileri getir

        Task AddAsync(T entity); // Yeni veri ekle
        void Update(T entity); // Veri güncelle
        void Delete(T entity); // Veri sil (soft delete için kullanılacak)
        Task<int> SaveAsync(); // Değişiklikleri kaydet

        // Diğer sorgu yardımcıları
        IQueryable<T> GetQuery();
        bool HasChange();
    }
}