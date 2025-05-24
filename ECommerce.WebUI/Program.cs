using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Interfaces.Storage;
using ECommerce.Application.Mapping;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Identity; // ApplicationUser modelimizin yolu
using ECommerce.Infrastructure.Persistence.Context;
using ECommerce.Infrastructure.ServiceRegistrations;
using ECommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity; // Identity için gerekli
using Microsoft.EntityFrameworkCore; // UseSqlServer için gerekli
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // IHostEnvironment için
using System;
using System.Threading.Tasks; // async/await için

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Oturum Verilerini Depolamak Ýçin Bellek Önbelleði Ekle
builder.Services.AddDistributedMemoryCache();

// 2. Oturum Servislerini Ekle
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HttpContextAccessor'ý WebUI katmanýnda kaydetmelisiniz.
// Sepet servisi gibi HttpContext'e ihtiyaç duyan servisler için gereklidir.
builder.Services.AddHttpContextAccessor();

// DbContext'i ekliyoruz. Identity için de bu DbContext kullanýlacak.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Infrastructure Katmanýndaki diðer servisleri ekliyoruz (Repositories, CartService, vb.)
builder.Services.AddInfrastructureServices(builder.Configuration);

// AutoMapper servislerini ekliyoruz
builder.Services.AddAutoMapper(typeof(GeneralMapping));

// --- IDENTITY SERVÝSLERÝNÝ EKLÝYORUZ ---
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Þifre Ayarlarý
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;

    // Kullanýcý Kilitlenme Ayarlarý
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Kullanýcý Ayarlarý
    options.User.RequireUniqueEmail = true;

    // Oturum Açma Ayarlarý
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// *** YENÝ EKLENECEK KISIM: Yetkilendirme için Cookie Ayarlarý ***
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Giriþ yapmayan kullanýcýlar buraya yönlendirilir
    options.AccessDeniedPath = "/Account/AccessDenied"; // Yetkisiz eriþim denemeleri buraya yönlendirilir
    options.SlidingExpiration = true; // Her istekte çerezin süresini uzat
    options.ExpireTimeSpan = TimeSpan.FromDays(30); // Çerez süresi 30 gün
});
// *** YENÝ EKLENECEK KISIM BÝTTÝ ***

// Identity UI için varsayýlan View'larý ve Controller'larý ekler
builder.Services.AddRazorPages();

// MVC Controller'larýný ve View'larýný ekliyoruz
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Oturum (Session) middleware'ini kullan.
app.UseSession();

// Kimlik doðrulama (Authentication) middleware'ini kullan.
app.UseAuthentication();

// Yetkilendirme (Authorization) middleware'ini kullan.
app.UseAuthorization();

// Razor Pages'leri map'liyoruz (Identity UI için).
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- ROLLERÝ VE ÝLK ADMIN KULLANICISINI OLUÞTURMA (Uygulama Baþlangýcýnda) ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>(); // appsettings okumak için

    // Admin Rolünü Oluþturma
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
        Console.WriteLine("Admin rolü oluþturuldu.");
    }

    // Customer Rolünü Oluþturma
    if (!await roleManager.RoleExistsAsync("Customer"))
    {
        await roleManager.CreateAsync(new IdentityRole("Customer"));
        Console.WriteLine("Customer rolü oluþturuldu.");
    }

    // Ýlk Admin Kullanýcýsýný Oluþturma
    var adminEmail = configuration["AdminSettings:Email"];
    var adminPassword = configuration["AdminSettings:Password"];

    if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
    {
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true // E-posta onayý varsayýlan olarak true
            };
            var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"Admin kullanýcýsý '{adminEmail}' oluþturuldu ve 'Admin' rolüne atandý.");
            }
            else
            {
                Console.WriteLine($"Hata: Admin kullanýcýsý oluþturulurken sorun oluþtu.");
                foreach (var error in createAdminResult.Errors)
                {
                    Console.WriteLine($"  - {error.Description}");
                }
            }
        }
        else
        {
            // Admin kullanýcý zaten varsa, rolünün atanýp atanmadýðýný kontrol et
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"Mevcut admin kullanýcýsý '{adminEmail}' 'Admin' rolüne atandý.");
            }
        }
    }
    else
    {
        Console.WriteLine("Uyarý: 'AdminSettings:Email' veya 'AdminSettings:Password' appsettings.json dosyasýnda tanýmlý deðil. Ýlk admin kullanýcý oluþturulamadý.");
    }
}
// --- ROLLERÝ VE ÝLK ADMIN KULLANICISINI OLUÞTURMA BÝTTÝ ---

app.Run();