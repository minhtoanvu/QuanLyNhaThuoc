using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

namespace QLNhaThuoc.Tests
{
    public class LoginUITests
    {
        [Fact]
        public async Task Login_VoiSaiMatKhau_HienThiThongBaoLoi()
        {
            // Yêu cầu: Web phải đang chạy ở localhost:5264
            // Arrange
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true // Chạy ngầm không hiện UI để test nhanh, để false nếu muốn xem bot click
            });
            
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            // Act
            await page.GotoAsync("http://localhost:5264/Account/Login");
            
            // Tìm và điền form
            await page.FillAsync("input[name='Username']", "admin");
            await page.FillAsync("input[name='Password']", "matkhausaibet");
            
            // Bấm nút đăng nhập
            await page.ClickAsync("button[type='submit']");
            
            // Chờ load trang
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Assert
            // Kiểm tra xem màn hình có hiện thông báo lỗi tài khoản hoặc mật khẩu không
            var errorText = await page.Locator(".text-danger, .alert-danger").InnerTextAsync();
            
            Assert.Contains("không đúng", errorText);
        }
    }
}
