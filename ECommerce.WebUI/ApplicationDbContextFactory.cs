using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System; // Environment sınıfı için

// DbContext'inizin bulunduğu namespace'i buraya ekleyin
using ECommerce.Infrastructure.Persistence;
using ECommerce.Infrastructure.Persistence.Context; // Örnek olarak

// Bu sınıf, ECommerce.WebUI projenizde veya bu proje tarafından referans alınan bir projede olmalıdır.
// ECommerce.WebUI, başlangıç projeniz olduğu için genellikle burada olması en uygunudur.
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // appsettings.json dosyasını yüklemek için bir yapılandırma oluşturucu (ConfigurationBuilder) kullanın.
        // Directory.GetCurrentDirectory() genellikle başlangıç projesinin çıktı dizini olur.
        // Eğer Factory başka bir projede ama WebUI startup ise Path.Combine kullanmanız gerekebilir.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            // Geliştirme ortamı için appsettings.Development.json gibi dosyaları da yükleyin
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();

        // appsettings.json'dan bağlantı dizesini alın
        // "DefaultConnection" kısmını appsettings.json'daki bağlantı dizesi adınızla değiştirin.
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Bağlantı dizgesi 'DefaultConnection' bulunamadı.");
        }

        // DbContextOptions'ı yapılandırın
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseSqlServer(connectionString,
            sqlServerOptionsAction: sqlOptions =>
            {
                // Migration'ların hangi derlemede (assembly) tutulacağını belirtin.
                // Genellikle DbContext'in bulunduğu derleme (assembly) olur.
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });

        // Yeni bir DbContext örneği döndürün
        return new ApplicationDbContext(builder.Options);
    }
}