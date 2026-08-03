using System;
using System.Collections.Generic;

namespace QLNhaThuoc.DTO
{
    public class NhapKhoDTO
    {
        public string MaKho { get; set; }
        public string MaNCC { get; set; }
        public string MaNV { get; set; }
        public List<NhapItemDTO> Items { get; set; }
    }

    public class NhapItemDTO
    {
        public string MaSP { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }
        public DateTime HanSuDung { get; set; }
    }
}
