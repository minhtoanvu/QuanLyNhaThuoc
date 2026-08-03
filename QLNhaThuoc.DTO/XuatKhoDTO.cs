using System.Collections.Generic;

namespace QLNhaThuoc.DTO
{
    public class XuatKhoDTO
    {
        public string MaKho { get; set; }
        public string MaNV { get; set; }
        public List<XuatItemDTO> Items { get; set; }
    }

    public class ExtractedXuatKhoDTO
    {
        public string MaKho { get; set; }
        public string MaNV { get; set; }
        public List<XuatItemDTO> Items { get; set; }
    }

    public class XuatItemDTO
    {
        public string MaSP { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaXuat { get; set; }
    }
}
