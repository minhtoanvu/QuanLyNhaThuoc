using System.ComponentModel.DataAnnotations;

namespace QLNhaThuoc.DTO
{
    public class NhaCungCap
    {
        [Key]
        [StringLength(20)]
        public string MaNCC { get; set; }

        [StringLength(100)]
        public string TenNCC { get; set; }

        [StringLength(15)]
        public string SoDT { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public string DiaChi { get; set; }
    }
}
