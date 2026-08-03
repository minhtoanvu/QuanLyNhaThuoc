using QLNhaThuoc.DTO;
using QLNhaThuoc.DAL;

namespace QLNhaThuoc.BUS
{
    public class KhachHangBUS
    {
        private readonly KhachHangDAL _dal;

        public KhachHangBUS(string connectionString)
        {
            _dal = new KhachHangDAL(connectionString);
        }

        public List<KhachHang> GetAll() => _dal.GetAll();

        public KhachHang GetById(string maKH) => _dal.GetById(maKH);

        public bool Insert(KhachHang kh)
        {
            if (string.IsNullOrEmpty(kh.MaKH))
                kh.MaKH = "KH" + DateTime.Now.ToString("yyMMddHHmmss");
            kh.DiemTichLuy = 0;
            kh.HangThanhVien = "Thành viên mới";
            return _dal.Insert(kh);
        }

        public bool Update(KhachHang kh) => _dal.Update(kh);

        public bool Delete(string maKH) => _dal.Delete(maKH);
    }
}
