using ECommerce.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ECommerce.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                // Projenizin kök dizinini belirleyin. Genellikle çözümün ana dizinidir.
                // Veya daha basitçe, WebUI projesinin bin/Debug/net8.0 klasörüne işaret edebiliriz.
                // Ancak bu, migration'ları çalıştırırken varsayılan proje olarak
                // Infrastructure'ı seçtiğimizde WebUI'ın appsettings.json'ını bulmak için karmaşıklık yaratır.
                // En iyi yol, dotnet CLI'ı kullanarak doğrudan WebUI projesi dizininde çalıştırmaktır.

                // Şimdilik, Package Manager Console'dan çalışırken bu factory'nin appsettings.json'ı bulması için
                // basit bir geçici çözüm olarak, ConfigurationBuilder'ı boş bırakıp
                // bağlantı dizesini doğrudan aşağıda manuel olarak verelim.
                // YA DA, WebUI projesinin appsettings.json'ının bulunduğu dizine doğru bir yolu inşa etmeye çalışın.
                // Bu genellikle şöyle olur:
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "ECommerce.WebUI"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlServer(connectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });

            return new ApplicationDbContext(builder.Options);
        }
    }
}