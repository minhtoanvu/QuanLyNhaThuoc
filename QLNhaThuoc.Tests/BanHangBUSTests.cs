using System;
using System.Collections.Generic;
using Xunit;
using QLNhaThuoc.BUS;
using QLNhaThuoc.DTO;

namespace QLNhaThuoc.Tests
{
    public class BanHangBUSTests
    {
        private readonly BanHangBUS _bus;

        public BanHangBUSTests()
        {
            // Truyền chuỗi kết nối giả vì hàm CalculateTotal không gọi DB
            _bus = new BanHangBUS("DummyConnectionString");
        }

        [Fact]
        public void CalculateTotal_VoiDanhSachRong_TraVeKhong()
        {
            // Arrange
            var items = new List<ChiTietHoaDon>();

            // Act
            var total = _bus.CalculateTotal(items, 0);

            // Assert
            Assert.Equal(0, total);
        }

        [Fact]
        public void CalculateTotal_VoiGiamGia10PhanTram_TraVeKetQuaDung()
        {
            // Arrange
            var items = new List<ChiTietHoaDon>
            {
                new ChiTietHoaDon { SoLuong = 2, DonGia = 50000 }, // 100k
                new ChiTietHoaDon { SoLuong = 1, DonGia = 100000 } // 100k
            };
            // Tổng là 200k, giảm 10% -> 180k

            // Act
            var total = _bus.CalculateTotal(items, 10);

            // Assert
            Assert.Equal(180000, total);
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(150)]
        public void CalculateTotal_VoiGiamGiaKhongHopLe_NemLoi(decimal discount)
        {
            // Arrange
            var items = new List<ChiTietHoaDon>
            {
                new ChiTietHoaDon { SoLuong = 1, DonGia = 100000 }
            };

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _bus.CalculateTotal(items, discount));
            Assert.Equal("Mức giảm giá không hợp lệ", ex.Message);
        }
        
        [Fact]
        public void CalculateTotal_VoiSoLuongAm_NemLoi()
        {
            // Arrange
            var items = new List<ChiTietHoaDon>
            {
                new ChiTietHoaDon { SoLuong = -5, DonGia = 10000 }
            };

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _bus.CalculateTotal(items, 0));
            Assert.Equal("Số lượng phải lớn hơn 0", ex.Message);
        }
    }
}
