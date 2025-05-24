using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Infrastructure.Persistence.Context; // ApplicationDbContext için
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks; // Task için

namespace ECommerce.Infrastructure.Repositories
{
    // T, BaseEntity'den miras alan bir sınıf olmalı
    // IRepository<T> arayüzünü implemente eder
    public class GenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context; // Doğru context değişkeni
        private readonly DbSet<T> _dbSet; // DbSet doğrudan erişim için

        // Tek ve doğru yapıcı metot (constructor)
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); // İlgili DbSet'i alırız
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            // Soft delete mantığı ApplicationDbContext'teki SaveChangesAsync'te yönetiliyor.
            // Burada sadece IsDeleted'i true yapıyoruz.
            entity.IsDeleted = true;
            _dbSet.Update(entity); // Durumu Modified olarak işaretler
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.Where(x => !x.IsDeleted).ToListAsync(); // Sadece silinmemiş kayıtları getir
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            // AsNoTracking: Sadece okunacak veriler için performans artışı sağlar, Entity Framework'ün takip etmesine gerek kalmaz.
        }

        public IQueryable<T> GetQuery()
        {
            return _dbSet.Where(x => !x.IsDeleted).AsQueryable(); // Soft delete'e uygun IQueryable döner
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(x => !x.IsDeleted).FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(x => !x.IsDeleted).Where(predicate).ToListAsync();
        }

        public bool HasChange()
        {
            // DbContext'te bekleyen değişiklik olup olmadığını kontrol eder
            return _context.ChangeTracker.HasChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}