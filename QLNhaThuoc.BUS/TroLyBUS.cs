using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using QLNhaThuoc.DAL;

namespace QLNhaThuoc.BUS
{
    public class TroLyBUS
    {
        private readonly TroLyDAL _dal;
        private readonly string _geminiApiKey;
        private readonly HttpClient _httpClient;

        public TroLyBUS(string connectionString, string geminiApiKey = "")
        {
            _dal = new TroLyDAL(connectionString);
            _geminiApiKey = geminiApiKey;
            _httpClient = new HttpClient();
        }

        public class TroLyResponse
        {
            public string Intent { get; set; } = "AI_RESPONSE";
            public string Message { get; set; }
            public bool Success { get; set; }
            public List<string> Suggestions { get; set; } = new List<string>();
        }

        public TroLyResponse XuLyCauHoi(string cauHoi)
        {
            // Sync wrapper for Controller compatibility
            try
            {
                return XuLyCauHoiAsync(cauHoi).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return new TroLyResponse
                {
                    Message = $"❌ Lỗi khi kết nối AI: {ex.Message}",
                    Success = false
                };
            }
        }

        public async Task<TroLyResponse> XuLyCauHoiAsync(string cauHoi)
        {
            if (string.IsNullOrWhiteSpace(cauHoi))
                return new TroLyResponse { Message = "Bạn chưa nhập câu hỏi.", Success = false };

            if (string.IsNullOrWhiteSpace(_geminiApiKey) || _geminiApiKey == "YOUR_GEMINI_API_KEY_HERE")
            {
                return new TroLyResponse
                {
                    Message = "⚠️ <strong>Thiếu API Key!</strong><br/>Hệ thống chưa được cấu hình Gemini API Key. Vui lòng mở file <code>appsettings.json</code> và thêm key của bạn vào mục <code>GeminiApiKey</code>.",
                    Success = false
                };
            }

            var tools = new JsonArray
            {
                new JsonObject
                {
                    ["functionDeclarations"] = new JsonArray
                    {
                        new JsonObject
                        {
                            ["name"] = "tra_cuu_ton_kho",
                            ["description"] = "Tra cứu số lượng tồn kho và thông tin chi tiết của một sản phẩm/thuốc.",
                            ["parameters"] = new JsonObject
                            {
                                ["type"] = "OBJECT",
                                ["properties"] = new JsonObject
                                {
                                    ["tenThuoc"] = new JsonObject { ["type"] = "STRING", ["description"] = "Tên thuốc cần tìm" }
                                },
                                ["required"] = new JsonArray { "tenThuoc" }
                            }
                        },
                        new JsonObject
                        {
                            ["name"] = "goi_y_thuoc",
                            ["description"] = "Tìm thuốc dựa trên triệu chứng bệnh hoặc công dụng mong muốn.",
                            ["parameters"] = new JsonObject
                            {
                                ["type"] = "OBJECT",
                                ["properties"] = new JsonObject
                                {
                                    ["trieuChung"] = new JsonObject { ["type"] = "STRING", ["description"] = "Triệu chứng hoặc công dụng (VD: đau họng, nhức đầu)" }
                                },
                                ["required"] = new JsonArray { "trieuChung" }
                            }
                        },
                        new JsonObject
                        {
                            ["name"] = "thong_ke_doanh_thu",
                            ["description"] = "Xem tổng doanh thu của nhà thuốc hôm nay hoặc tháng này.",
                            ["parameters"] = new JsonObject
                            {
                                ["type"] = "OBJECT",
                                ["properties"] = new JsonObject
                                {
                                    ["loai"] = new JsonObject { ["type"] = "STRING", ["description"] = "hom_nay hoặc thang_nay" }
                                }
                            }
                        }
                    }
                }
            };

            var systemInstruction = new JsonObject
            {
                ["parts"] = new JsonArray
                {
                    new JsonObject { ["text"] = "Bạn là PharmaAI, dược sĩ tư vấn thông minh của hệ thống Quản lý nhà thuốc. Bạn LUÔN LUÔN định dạng câu trả lời bằng HTML (dùng thẻ <strong>, <br>, <ul>, <li>, <span style='color:red'>...) để hiển thị đẹp trên Web thay vì dùng Markdown. Bạn có khả năng truy xuất dữ liệu nhà thuốc qua các công cụ. Hãy tư vấn nhiệt tình, thân thiện và trả lời đúng trọng tâm. Nếu người dùng hỏi triệu chứng, hãy luôn dùng công cụ goi_y_thuoc để lấy thông tin." }
                }
            };

            var contents = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "user",
                    ["parts"] = new JsonArray { new JsonObject { ["text"] = cauHoi } }
                }
            };

            var requestBody = new JsonObject
            {
                ["systemInstruction"] = systemInstruction,
                ["contents"] = contents,
                ["tools"] = tools
            };

            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_geminiApiKey}";

            // Loop for function calling (max 3 turns)
            for (int i = 0; i < 3; i++)
            {
                var responseJson = await SendGeminiRequest(url, requestBody);
                var candidates = responseJson["candidates"]?.AsArray();
                
                if (candidates == null || candidates.Count == 0)
                    throw new Exception("Không nhận được phản hồi từ AI.");

                var firstCandidate = candidates[0];
                var parts = firstCandidate["content"]?["parts"]?.AsArray();
                var functionCall = parts?.FirstOrDefault(p => p["functionCall"] != null)?["functionCall"];

                if (functionCall != null)
                {
                    // Call local function
                    string functionName = functionCall["name"].ToString();
                    var args = functionCall["args"];
                    
                    var functionResult = ExecuteTool(functionName, args);

                    // Add model response (the function call) to history
                    contents.Add(firstCandidate["content"].DeepClone());

                    // Add function response to history
                    contents.Add(new JsonObject
                    {
                        ["role"] = "user",
                        ["parts"] = new JsonArray
                        {
                            new JsonObject
                            {
                                ["functionResponse"] = new JsonObject
                                {
                                    ["name"] = functionName,
                                    ["response"] = new JsonObject { ["name"] = functionName, ["content"] = functionResult }
                                }
                            }
                        }
                    });

                    // Loop continues, sending the updated contents to Gemini
                }
                else
                {
                    // We got final text response
                    var text = parts?.FirstOrDefault(p => p["text"] != null)?["text"]?.ToString();
                    return new TroLyResponse
                    {
                        Message = text?.Replace("\n", "<br>") ?? "Xin lỗi, tôi không thể trả lời lúc này.",
                        Success = true,
                        Suggestions = new List<string> { "Đau họng uống thuốc gì?", "Doanh thu hôm nay" } // Default suggestions
                    };
                }
            }

            return new TroLyResponse { Message = "Hệ thống AI đang bận xử lý quá nhiều tác vụ, vui lòng thử lại.", Success = false };
        }

        private JsonNode ExecuteTool(string functionName, JsonNode args)
        {
            try
            {
                if (functionName == "tra_cuu_ton_kho")
                {
                    string ten = args["tenThuoc"]?.ToString() ?? "";
                    var sp = _dal.GetTonKho(ten);
                    if (sp != null)
                        return JsonSerializer.SerializeToNode(sp);
                    return JsonSerializer.SerializeToNode(new { error = "Không tìm thấy sản phẩm" });
                }
                else if (functionName == "goi_y_thuoc")
                {
                    string trieuChung = args["trieuChung"]?.ToString() ?? "";
                    var list = _dal.GoiYThuocTheoTrieuChung(trieuChung);
                    return JsonSerializer.SerializeToNode(list);
                }
                else if (functionName == "thong_ke_doanh_thu")
                {
                    string loai = args["loai"]?.ToString() ?? "hom_nay";
                    if (loai == "thang_nay")
                    {
                        var dtThang = _dal.GetDoanhThuThang();
                        return JsonSerializer.SerializeToNode(new { DoanhThuThang = dtThang });
                    }
                    else
                    {
                        var dtNay = _dal.GetDoanhThuHomNay();
                        return JsonSerializer.SerializeToNode(new { DoanhThuHomNay = dtNay.doanhThu, SoDon = dtNay.soDon });
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.SerializeToNode(new { error = ex.Message });
            }
            
            return JsonSerializer.SerializeToNode(new { error = "Công cụ không tồn tại" });
        }

        private async Task<JsonObject> SendGeminiRequest(string url, JsonObject requestBody)
        {
            var content = new StringContent(requestBody.ToJsonString(), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            string responseString = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Gemini API Error: {responseString}");
            }
            
            return JsonNode.Parse(responseString).AsObject();
        }
    }
}
