using QLNhaThuoc.DAL;
using QLNhaThuoc.DTO;

namespace QLNhaThuoc.BUS
{
    public class KhoBUS
    {
        private readonly KhoDAL _dal;

        public KhoBUS(string connectionString)
        {
            _dal = new KhoDAL(connectionString);
        }

        public Task<(List<KhoDTO> kho, List<NccDTO> ncc, List<SpDTO> sp)> GetDanhSachNhap()
            => _dal.GetDanhSachNhap();

        public Task<string> ProcessNhapKho(NhapKhoDTO data)
            => _dal.ProcessNhapKho(data);

        public Task<string> ProcessXuatKho(XuatKhoDTO data)
            => _dal.ProcessXuatKho(data);

        public PhieuNhapInfoDTO GetPhieuNhapInfo(string maPNK)
            => _dal.GetPhieuNhapInfo(maPNK);

        public PhieuXuatInfoDTO GetPhieuXuatInfo(string maPXK)
            => _dal.GetPhieuXuatInfo(maPXK);
    }
}
