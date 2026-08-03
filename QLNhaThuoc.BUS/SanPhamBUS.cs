using QLNhaThuoc.DTO;
using QLNhaThuoc.DAL;

namespace QLNhaThuoc.BUS
{
    public class SanPhamBUS
    {
        private readonly SanPhamDAL _dal;
        private readonly DanhMucDAL _dmDal;

        public SanPhamBUS(string connectionString)
        {
            _dal = new SanPhamDAL(connectionString);
            _dmDal = new DanhMucDAL(connectionString);
        }

        public List<SanPham> GetAll() => _dal.GetAll();

        public SanPham GetById(string maSP) => _dal.GetById(maSP);

        public List<DanhMuc> GetDanhMucList() => _dmDal.GetAll();

        public bool Insert(SanPham sp) => _dal.Insert(sp);

        public bool Update(SanPham sp) => _dal.Update(sp);

        public bool Delete(string maSP) => _dal.Delete(maSP);
    }
}
