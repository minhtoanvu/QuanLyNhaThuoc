using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using QLNhaThuoc.BUS;

namespace QLNhaThuocApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly NhanVienBUS _bus;

        public AccountController(IConfiguration configuration)
        {
            _bus = new NhanVienBUS(configuration.GetConnectionString("DefaultConnection"));
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string tenDangNhap, string matKhau, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ Tài khoản và Mật khẩu.");
                return View();
            }

            try
            {
                var nv = _bus.DangNhap(tenDangNhap, matKhau);
                if (nv != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, nv.MaNV),
                        new Claim(ClaimTypes.Name, nv.Ho + " " + nv.Ten),
                        new Claim(ClaimTypes.Role, nv.ChucVu)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi kết nối CSDL: " + ex.Message);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}
