using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QLNhaThuoc.DTO;
using QLNhaThuoc.BUS;

namespace QLNhaThuocApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DanhMucController : Controller
    {
        private readonly DanhMucBUS _bus;

        public DanhMucController(IConfiguration configuration)
        {
            _bus = new DanhMucBUS(configuration.GetConnectionString("DefaultConnection"));
        }

        public IActionResult Index()
        {
            try { return View(_bus.GetAll()); }
            catch (Exception ex) { ViewBag.ErrorMessage = "Lỗi: " + ex.Message; return View(new List<DanhMuc>()); }
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DanhMuc dm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _bus.Insert(dm);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { ModelState.AddModelError("", "Không thể lưu dữ liệu: " + ex.Message); }
            }
            return View(dm);
        }

        public IActionResult Edit(string id)
        {
            if (id == null) return NotFound();
            var dm = _bus.GetById(id);
            if (dm == null) return NotFound();
            return View(dm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, DanhMuc dm)
        {
            if (id != dm.MaDM) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _bus.Update(dm);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { ModelState.AddModelError("", "Không thể cập nhật: " + ex.Message); }
            }
            return View(dm);
        }

        public IActionResult Delete(string id)
        {
            if (id == null) return NotFound();
            var dm = _bus.GetById(id);
            if (dm == null) return NotFound();
            return View(dm);
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