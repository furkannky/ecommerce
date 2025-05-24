using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.Account; // DTO'ları kullanmak için ekle
using ECommerce.Domain.Entities.Identity; // ApplicationUser'ı kullanmak için ekle

namespace ECommerce.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // *** YENİ EKLENECEK KISIM: Kayıt olan kullanıcıya varsayılan "Customer" rolünü ata ***
                    var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                    if (!roleResult.Succeeded)
                    {
                        // Rol ataması başarısız olursa hataları ekle
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        // Rol ataması başarısız olursa kullanıcıyı sil (isteğe bağlı, duruma göre değişir)
                        // await _userManager.DeleteAsync(user);
                        return View(model); // Tekrar kayıt formuna yönlendir
                    }
                    // *** YENİ EKLENECEK KISIM BİTTİ ***

                    await _signInManager.SignInAsync(user, isPersistent: false); // Kullanıcıyı otomatik giriş yap
                    TempData["SuccessMessage"] = "Kaydınız başarıyla tamamlandı!";
                    return RedirectToAction("Index", "Product"); // Kayıt sonrası ana sayfaya yönlendir
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Email'i UserName olarak kullanıyoruz
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Giriş başarılı!";

                    // *** YENİ EKLENECEK KISIM: Admin girişi ise Admin paneline yönlendir ***
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        TempData["SuccessMessage"] = "Admin olarak giriş başarılı!";
                        return RedirectToAction("Index", "AdminDashboard"); // Admin paneli kontrolörünüz varsa
                    }
                    // *** YENİ EKLENECEK KISIM BİTTİ ***

                    return RedirectToLocal(returnUrl); // Giriş sonrası istenen sayfaya veya ana sayfaya yönlendir
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Hesabınız kilitlendi. Lütfen daha sonra tekrar deneyin.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
                }
            }
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Çıkış başarılı.";
            return RedirectToAction("Index", "Product"); // Çıkış sonrası ana sayfaya yönlendir
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Product");
            }
        }

        // *** YENİ EKLENECEK KISIM: Yetkisiz erişim için AccessDenied sayfası ***
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        // *** YENİ EKLENECEK KISIM BİTTİ ***
    }
}