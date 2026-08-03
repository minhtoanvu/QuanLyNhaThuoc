using System.ComponentModel.DataAnnotations;

namespace QLNhaThuoc.DTO
{
    public class ChiTietNhapKho
    {
        [StringLength(20)]
        public string MaPNK { get; set; }

        [StringLength(20)]
        public string MaSP { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGiaNhap { get; set; }

        public DateTime HanSuDung { get; set; }
    }
}
