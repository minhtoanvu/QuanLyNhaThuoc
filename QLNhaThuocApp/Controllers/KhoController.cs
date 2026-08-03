using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using QLNhaThuoc.BUS;
using QLNhaThuoc.DTO;

namespace QLNhaThuocApp.Controllers
{
    [Authorize(Roles = "Thủ Kho, Admin")]
    public class KhoController : Controller
    {
        private readonly KhoBUS _bus;

        public KhoController(IConfiguration config)
        {
            _bus = new KhoBUS(config.GetConnectionString("DefaultConnection"));
        }

        public IActionResult NhapKho() => View();
        public IActionResult XuatKho() => View();

        [HttpGet]
        public async Task<IActionResult> GetDanhSachNhap()
        {
            var (kho, ncc, sp) = await _bus.GetDanhSachNhap();
            return Json(new { kho, ncc, sp });
        }

        public class NhapKhoPayload
        {
            public string MaKho { get; set; }
            public string MaNCC { get; set; }
            public List<NhapItem> Items { get; set; }
        }

        public class NhapItem
        {
            public string MaSP { get; set; }
            public int SoLuong { get; set; }
            public decimal DonGiaNhap { get; set; }
            public DateTime HanSuDung { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessNhapKho([FromBody] NhapKhoPayload payload)
        {
            if (payload.Items == null || !payload.Items.Any())
                return BadRequest(new { success = false, message = "Vui lòng thêm sản phẩm." });

            try
            {
                string maNV = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var data = new NhapKhoDTO
                {
                    MaKho = payload.MaKho,
                    MaNCC = payload.MaNCC,
                    MaNV = maNV,
                    Items = payload.Items.Select(i => new NhapItemDTO
                    {
                        MaSP = i.MaSP,
                        SoLuong = i.SoLuong,
                        DonGiaNhap = i.DonGiaNhap,
                        HanSuDung = i.HanSuDung
                    }).ToList()
                };

                string maPNK = await _bus.ProcessNhapKho(data);
                return Json(new { success = true, maPNK });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        public class XuatKhoPayload
        {
            public string MaKho { get; set; }
            public List<XuatItem> Items { get; set; }
        }

        public class XuatItem
        {
            public string MaSP { get; set; }
            public int SoLuong { get; set; }
            public decimal GiaXuat { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessXuatKho([FromBody] XuatKhoPayload payload)
        {
            if (payload.Items == null || !payload.Items.Any())
                return BadRequest(new { success = false, message = "Vui lòng chọn sản phẩm xuất." });

            try
            {
                string maNV = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var data = new XuatKhoDTO
                {
                    MaKho = payload.MaKho,
                    MaNV = maNV,
                    Items = payload.Items.Select(i => new XuatItemDTO
                    {
                        MaSP = i.MaSP,
                        SoLuong = i.SoLuong,
                        GiaXuat = i.GiaXuat
                    }).ToList()
                };

                string maPXK = await _bus.ProcessXuatKho(data);
                return Json(new { success = true, maPXK });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        public IActionResult XemPhieuNhap(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var phieuNhap = _bus.GetPhieuNhapInfo(id);
            if (phieuNhap == null) return NotFound();
            return View(phieuNhap);
        }

        public IActionResult XemPhieuXuat(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var phieuXuat = _bus.GetPhieuXuatInfo(id);
            if (phieuXuat == null) return NotFound();
            return View(phieuXuat);
        }
    }
}