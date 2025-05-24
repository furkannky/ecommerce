using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Interfaces.Storage;
using ECommerce.Application.Mapping;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Identity; // ApplicationUser modelimizin yolu
using ECommerce.Infrastructure.Persistence.Context;
using ECommerce.Infrastructure.ServiceRegistrations;
using ECommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity; // Identity i�in gerekli
using Microsoft.EntityFrameworkCore; // UseSqlServer i�in gerekli
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // IHostEnvironment i�in
using System;
using System.Threading.Tasks; // async/await i�in

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Oturum Verilerini Depolamak ��in Bellek �nbelle�i Ekle
builder.Services.AddDistributedMemoryCache();

// 2. Oturum Servislerini Ekle
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HttpContextAccessor'� WebUI katman�nda kaydetmelisiniz.
// Sepet servisi gibi HttpContext'e ihtiya� duyan servisler i�in gereklidir.
builder.Services.AddHttpContextAccessor();

// DbContext'i ekliyoruz. Identity i�in de bu DbContext kullan�lacak.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Infrastructure Katman�ndaki di�er servisleri ekliyoruz (Repositories, CartService, vb.)
builder.Services.AddInfrastructureServices(builder.Configuration);

// AutoMapper servislerini ekliyoruz
builder.Services.AddAutoMapper(typeof(GeneralMapping));

// --- IDENTITY SERV�SLER�N� EKL�YORUZ ---
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // �ifre Ayarlar�
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;

    // Kullan�c� Kilitlenme Ayarlar�
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Kullan�c� Ayarlar�
    options.User.RequireUniqueEmail = true;

    // Oturum A�ma Ayarlar�
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// *** YEN� EKLENECEK KISIM: Yetkilendirme i�in Cookie Ayarlar� ***
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Giri� yapmayan kullan�c�lar buraya y�nlendirilir
    options.AccessDeniedPath = "/Account/AccessDenied"; // Yetkisiz eri�im denemeleri buraya y�nlendirilir
    options.SlidingExpiration = true; // Her istekte �erezin s�resini uzat
    options.ExpireTimeSpan = TimeSpan.FromDays(30); // �erez s�resi 30 g�n
});
// *** YEN� EKLENECEK KISIM B�TT� ***

// Identity UI i�in varsay�lan View'lar� ve Controller'lar� ekler
builder.Services.AddRazorPages();

// MVC Controller'lar�n� ve View'lar�n� ekliyoruz
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

// Kimlik do�rulama (Authentication) middleware'ini kullan.
app.UseAuthentication();

// Yetkilendirme (Authorization) middleware'ini kullan.
app.UseAuthorization();

// Razor Pages'leri map'liyoruz (Identity UI i�in).
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- ROLLER� VE �LK ADMIN KULLANICISINI OLU�TURMA (Uygulama Ba�lang�c�nda) ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>(); // appsettings okumak i�in

    // Admin Rol�n� Olu�turma
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
        Console.WriteLine("Admin rol� olu�turuldu.");
    }

    // Customer Rol�n� Olu�turma
    if (!await roleManager.RoleExistsAsync("Customer"))
    {
        await roleManager.CreateAsync(new IdentityRole("Customer"));
        Console.WriteLine("Customer rol� olu�turuldu.");
    }

    // �lk Admin Kullan�c�s�n� Olu�turma
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
                EmailConfirmed = true // E-posta onay� varsay�lan olarak true
            };
            var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"Admin kullan�c�s� '{adminEmail}' olu�turuldu ve 'Admin' rol�ne atand�.");
            }
            else
            {
                Console.WriteLine($"Hata: Admin kullan�c�s� olu�turulurken sorun olu�tu.");
                foreach (var error in createAdminResult.Errors)
                {
                    Console.WriteLine($"  - {error.Description}");
                }
            }
        }
        else
        {
            // Admin kullan�c� zaten varsa, rol�n�n atan�p atanmad���n� kontrol et
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"Mevcut admin kullan�c�s� '{adminEmail}' 'Admin' rol�ne atand�.");
            }
        }
    }
    else
    {
        Console.WriteLine("Uyar�: 'AdminSettings:Email' veya 'AdminSettings:Password' appsettings.json dosyas�nda tan�ml� de�il. �lk admin kullan�c� olu�turulamad�.");
    }
}
// --- ROLLER� VE �LK ADMIN KULLANICISINI OLU�TURMA B�TT� ---

app.Run();