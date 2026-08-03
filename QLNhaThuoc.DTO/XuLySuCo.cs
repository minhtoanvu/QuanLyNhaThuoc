using System.ComponentModel.DataAnnotations;

namespace QLNhaThuoc.DTO
{
    public class XuLySuCo
    {
        [StringLength(20)]
        public string MaBCSC { get; set; }

        public DateTime NgayXuLy { get; set; }

        public string KetQua { get; set; }

        [StringLength(20)]
        public string MaNV { get; set; }
    }
}
