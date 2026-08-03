using QLNhaThuoc.DTO;
using QLNhaThuoc.DAL;

namespace QLNhaThuoc.BUS
{
    public class SuCoBUS
    {
        private readonly SuCoDAL _dal;

        public SuCoBUS(string connectionString)
        {
            _dal = new SuCoDAL(connectionString);
        }

        public List<BaoCaoSuCo> GetAll() => _dal.GetAll();

        public SuCoDAL.ChiTietSuCoDTO GetChiTiet(string maBCSC) => _dal.GetChiTiet(maBCSC);

        public List<object> GetDanhSachKhachHang() => _dal.GetDanhSachKhachHang();

        public bool Insert(string maKH, string maHD, string noiDung)
        {
            var sc = new BaoCaoSuCo
            {
                MaBCSC = "SC" + DateTime.Now.ToString("yyMMddHHmmss"),
                MaKH = string.IsNullOrEmpty(maKH) ? null : maKH,
                MaHD = string.IsNullOrEmpty(maHD) ? null : maHD,
                NgayGui = DateTime.Now,
                NoiDung = noiDung,
                TrangThai = "Chờ xử lý"
            };
            return _dal.Insert(sc);
        }

        public bool XuLy(string maBCSC, string ketQua, string maNV, string action)
        {
            string trangThai = action == "Resolve" ? "Đã xác nhận" : "Đã hủy";
            return _dal.XuLy(maBCSC, ketQua, maNV, trangThai);
        }
    }
}
