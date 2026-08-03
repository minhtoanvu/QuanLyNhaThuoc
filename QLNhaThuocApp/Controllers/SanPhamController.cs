using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLNhaThuoc.DTO;
using QLNhaThuoc.BUS;

namespace QLNhaThuocApp.Controllers
{
    [Authorize(Roles = "Thủ Kho, Admin")]
    public class SanPhamController : Controller
    {
        private readonly SanPhamBUS _bus;

        public SanPhamController(IConfiguration configuration)
        {
            _bus = new SanPhamBUS(configuration.GetConnectionString("DefaultConnection"));
        }

        public IActionResult Index()
        {
            try { return View(_bus.GetAll()); }
            catch (Exception ex) { ViewBag.ErrorMessage = "Lỗi: " + ex.Message; return View(new List<SanPham>()); }
        }

        private void PopulateDanhMucDropDown(object selected = null)
        {
            var danhMucs = _bus.GetDanhMucList()
                .Select(dm => new SelectListItem { Value = dm.MaDM, Text = dm.TenDM });
            ViewBag.MaDM = new SelectList(danhMucs, "Value", "Text", selected);
        }

        public IActionResult Create()
        {
            PopulateDanhMucDropDown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SanPham sp)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _bus.Insert(sp);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { ModelState.AddModelError("", "Không thể lưu dữ liệu: " + ex.Message); }
            }
            PopulateDanhMucDropDown(sp.MaDM);
            return View(sp);
        }

        public IActionResult Edit(string id)
        {
            if (id == null) return NotFound();
            var sp = _bus.GetById(id);
            if (sp == null) return NotFound();
            PopulateDanhMucDropDown(sp.MaDM);
            return View(sp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, SanPham sp)
        {
            if (id != sp.MaSP) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _bus.Update(sp);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { ModelState.AddModelError("", "Không thể cập nhật: " + ex.Message); }
            }
            PopulateDanhMucDropDown(sp.MaDM);
            return View(sp);
        }

        public IActionResult Delete(string id)
        {
            if (id == null) return NotFound();
            var sp = _bus.GetById(id);
            if (sp == null) return NotFound();
            return View(sp);
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
