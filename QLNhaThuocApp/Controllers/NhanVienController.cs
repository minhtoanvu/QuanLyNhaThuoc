using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QLNhaThuoc.DTO;
using QLNhaThuoc.BUS;

namespace QLNhaThuocApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NhanVienController : Controller
    {
        private readonly NhanVienBUS _bus;

        public NhanVienController(IConfiguration configuration)
        {
            _bus = new NhanVienBUS(configuration.GetConnectionString("DefaultConnection"));
        }

        public IActionResult Index()
        {
            try { return View(_bus.GetAll()); }
            catch (Exception ex) { ViewBag.ErrorMessage = "Lỗi: " + ex.Message; return View(new List<NhanVien>()); }
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NhanVien nv)
        {
            if (string.IsNullOrEmpty(nv.ChucVu))
                ModelState.AddModelError("ChucVu", "Vui lòng chọn chức vụ.");
            if (string.IsNullOrEmpty(nv.TenDangNhap))
                ModelState.AddModelError("TenDangNhap", "Vui lòng nhập tên đăng nhập.");
            if (string.IsNullOrEmpty(nv.MatKhau))
                ModelState.AddModelError("MatKhau", "Vui lòng nhập mật khẩu.");

            if (ModelState.IsValid)
            {
                try
                {
                    if (!_bus.Insert(nv, out string error))
                    {
                        ModelState.AddModelError("TenDangNhap", error);
                        return View(nv);
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { ModelState.AddModelError("", "Không thể lưu dữ liệu: " + ex.Message); }
            }
            return View(nv);
        }

        public IActionResult Edit(string id)
        {
            if (id == null) return NotFound();
            var nv = _bus.GetById(id);
            if (nv == null) return NotFound();
            nv.MatKhau = "HIDDEN";
            return View(nv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, NhanVien nv, string MatKhauMoi)
        {
            if (id != nv.MaNV) return NotFound();
            if (string.IsNullOrEmpty(nv.ChucVu))
                ModelState.AddModelError("ChucVu", "Vui lòng chọn chức vụ.");

            nv.MatKhau = "TEMP";
            ModelState.Remove("MatKhau");

            if (ModelState.IsValid)
            {
                try
                {
                    _bus.Update(nv, MatKhauMoi);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { ModelState.AddModelError("", "Không thể cập nhật: " + ex.Message); }
            }
            return View(nv);
        }

        public IActionResult Delete(string id)
        {
            if (id == null) return NotFound();
            var nv = _bus.GetById(id);
            if (nv == null) return NotFound();
            return View(nv);
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
                TempData["ErrorMessage"] = "Không thể xóa nhân viên: " + ex.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}