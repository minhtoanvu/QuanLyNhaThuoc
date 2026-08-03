using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QLNhaThuoc.DTO;
using QLNhaThuoc.BUS;

namespace QLNhaThuocApp.Controllers
{
    [Authorize(Roles = "Bán Hàng, Admin")]
    public class KhachHangController : Controller
    {
        private readonly KhachHangBUS _bus;

        public KhachHangController(IConfiguration configuration)
        {
            _bus = new KhachHangBUS(configuration.GetConnectionString("DefaultConnection"));
        }

        public IActionResult Index()
        {
            try { return View(_bus.GetAll()); }
            catch (Exception ex) { ViewBag.ErrorMessage = "Lỗi: " + ex.Message; return View(new List<KhachHang>()); }
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _bus.Insert(kh);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { ModelState.AddModelError("", "Không thể lưu dữ liệu: " + ex.Message); }
            }
            return View(kh);
        }

        public IActionResult Edit(string id)
        {
            if (id == null) return NotFound();
            var kh = _bus.GetById(id);
            if (kh == null) return NotFound();
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, KhachHang kh)
        {
            if (id != kh.MaKH) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _bus.Update(kh);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { ModelState.AddModelError("", "Không thể cập nhật: " + ex.Message); }
            }
            return View(kh);
        }

        public IActionResult Delete(string id)
        {
            if (id == null) return NotFound();
            var kh = _bus.GetById(id);
            if (kh == null) return NotFound();
            return View(kh);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            try
            {
                _bus.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể xóa: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}