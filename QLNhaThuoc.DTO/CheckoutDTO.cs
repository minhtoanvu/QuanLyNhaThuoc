using System.Collections.Generic;

namespace QLNhaThuoc.DTO
{
    public class CheckoutDTO
    {
        public string MaKH { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public int DiemSuDung { get; set; }
        public decimal TienGiamTru { get; set; }
        public string MaNV { get; set; }
        public List<CartItemDTO> Items { get; set; }
    }

    public class CartItemDTO
    {
        public string MaSP { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
