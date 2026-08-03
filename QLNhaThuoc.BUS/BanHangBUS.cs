using QLNhaThuoc.DTO;
using QLNhaThuoc.DAL;

namespace QLNhaThuoc.BUS
{
    public class BanHangBUS
    {
        private readonly BanHangDAL _dal;

        public BanHangBUS(string connectionString)
        {
            _dal = new BanHangDAL(connectionString);
        }

        public List<SanPhamBanDTO> GetSanPhamBan() => _dal.GetSanPhamBan();

        public List<KhachHangBanDTO> GetKhachHangBan() => _dal.GetKhachHangBan();

        public List<HoaDon> GetLichSuHoaDon() => _dal.GetLichSuHoaDon();

        public List<ChiTietHoaDon> GetChiTietHoaDon(string maHD) => _dal.GetChiTietHoaDon(maHD);

        public Task<string> Checkout(CheckoutDTO data) => _dal.Checkout(data);

        public (HoaDon, List<ChiTietHoaDon>) GetHoaDonInfo(string maHD) => _dal.GetHoaDonInfo(maHD);

        // Logic tính tổng tiền để phục vụ cho Automation Unit Test
        public decimal CalculateTotal(List<ChiTietHoaDon> items, decimal discountPercentage)
        {
            if (items == null || items.Count == 0) return 0;
            if (discountPercentage < 0 || discountPercentage > 100)
                throw new ArgumentException("Mức giảm giá không hợp lệ");

            decimal total = 0;
            foreach (var item in items)
            {
                if (item.SoLuong <= 0) throw new ArgumentException("Số lượng phải lớn hơn 0");
                total += item.SoLuong * item.DonGia;
            }

            return total - (total * (discountPercentage / 100));
        }
    }
}
