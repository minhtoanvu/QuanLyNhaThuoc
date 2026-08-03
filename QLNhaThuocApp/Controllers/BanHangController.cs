using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QLNhaThuoc.BUS;
using QLNhaThuoc.DTO;
using System.Security.Claims;

namespace QLNhaThuocApp.Controllers
{
    [Authorize(Roles = "Bán Hàng, Admin")]
    public class BanHangController : Controller
    {
        private readonly BanHangBUS _bus;

        public BanHangController(IConfiguration configuration)
        {
            _bus = new BanHangBUS(configuration.GetConnectionString("DefaultConnection"));
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Bán Mới - Lập Hóa Đơn";
            return View();
        }

        public IActionResult LichSu()
        {
            ViewBag.Title = "Lịch Sử Hóa Đơn";
            return View(_bus.GetLichSuHoaDon());
        }

        public IActionResult ChiTietHoadon(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            ViewBag.MaHD = id;
            return PartialView("_ChiTietHoaDon", _bus.GetChiTietHoaDon(id));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetSanPhamList() => Json(_bus.GetSanPhamBan());

        [HttpGet]
        public IActionResult GetKhachHangList() => Json(_bus.GetKhachHangBan());

        // DTO nhận từ JSON (giữ lại để tương thích với JS frontend)
        public class CheckoutPayload
        {
            public string MaKH { get; set; }
            public string PhuongThucThanhToan { get; set; }
            public int DiemSuDung { get; set; }
            public decimal TienGiamTru { get; set; }
            public List<CartItem> Items { get; set; }
        }

        public class CartItem
        {
            public string MaSP { get; set; }
            public int SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckoutPayload payload)
        {
            if (payload == null || payload.Items == null || !payload.Items.Any())
                return BadRequest(new { success = false, message = "Giỏ hàng rỗng." });

            string maNV = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNV))
                return Unauthorized(new { success = false, message = "Vui lòng đăng nhập." });

            try
            {
                var data = new CheckoutDTO
                {
                    MaKH = payload.MaKH,
                    PhuongThucThanhToan = payload.PhuongThucThanhToan,
                    DiemSuDung = payload.DiemSuDung,
                    TienGiamTru = payload.TienGiamTru,
                    MaNV = maNV,
                    Items = payload.Items.Select(i => new CartItemDTO
                    {
                        MaSP = i.MaSP,
                        SoLuong = i.SoLuong,
                        DonGia = i.DonGia,
                        ThanhTien = i.ThanhTien
                    }).ToList()
                };

                string maHD = await _bus.Checkout(data);
                return Json(new { success = true, maHD, message = "Tạo hóa đơn thành công!", redirectUrl = Url.Action("XemHoaDon", "BanHang", new { id = maHD }) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        public IActionResult XemHoaDon(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var (hoaDon, chiTiet) = _bus.GetHoaDonInfo(id);
            if (hoaDon == null) return NotFound();
            ViewBag.ChiTiet = chiTiet;
            return View(hoaDon);
        }
    }
}