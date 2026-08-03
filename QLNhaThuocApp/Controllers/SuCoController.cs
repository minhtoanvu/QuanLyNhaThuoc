using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using QLNhaThuoc.BUS;

namespace QLNhaThuocApp.Controllers
{
    [Authorize(Roles = "Bán Hàng, Admin")]
    public class SuCoController : Controller
    {
        private readonly SuCoBUS _bus;

        public SuCoController(IConfiguration configuration)
        {
            _bus = new SuCoBUS(configuration.GetConnectionString("DefaultConnection"));
        }

        public IActionResult Index()
        {
            try { return View(_bus.GetAll()); }
            catch (Exception ex) { ViewBag.ErrorMessage = "Lỗi: " + ex.Message; return View(new List<QLNhaThuoc.DTO.BaoCaoSuCo>()); }
        }

        public IActionResult Create()
        {
            var khs = _bus.GetDanhSachKhachHang()
                .Select(k => new SelectListItem
                {
                    Value = ((dynamic)k).maKH,
                    Text = ((dynamic)k).tenKH
                }).ToList();
            ViewBag.MaKH = new SelectList(khs, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string MaKH, string MaHD, string NoiDung)
        {
            if (string.IsNullOrEmpty(NoiDung))
            {
                ModelState.AddModelError("", "Nội dung báo cáo không được để trống.");
                return RedirectToAction("Create");
            }

            try
            {
                _bus.Insert(MaKH, MaHD, NoiDung);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                return RedirectToAction("Create");
            }
        }

        public IActionResult Process(string id)
        {
            if (id == null) return NotFound();
            var vm = _bus.GetChiTiet(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitProcess(string MaBCSC, string KetQua, string Action)
        {
            if (string.IsNullOrEmpty(KetQua))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập kết quả xử lý.";
                return RedirectToAction("Process", new { id = MaBCSC });
            }

            string maNV = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                _bus.XuLy(MaBCSC, KetQua, maNV, Action);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi lưu xử lý: " + ex.Message;
                return RedirectToAction("Process", new { id = MaBCSC });
            }
        }
    }
}