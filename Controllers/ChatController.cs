namespace WebsiteBanHang.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Linq;
    using WebsiteBanHang.Service;

    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly DialogflowService _dialogflowService;

        public ChatController(DialogflowService dialogflowService)
        {
            _dialogflowService = dialogflowService;
        }

        [HttpPost("ask")]
        public IActionResult Ask([FromBody] QueryModel queryModel)
        {
            if (queryModel == null || string.IsNullOrEmpty(queryModel.Query))
            {
                return BadRequest(new { error = "Query is required and cannot be empty." });
            }

            try
            {
                // Tạo session ID duy nhất (có thể dựa vào userId hoặc một phương thức khác nếu cần)
                var sessionId = "unique-session-id";

                // Gọi dịch vụ Dialogflow (đồng bộ)
                var response = _dialogflowService.DetectIntentAsync(sessionId, queryModel.Query);

                // Trả về phản hồi từ Dialogflow
                return Ok(new { message = response });
            }
            catch (Exception ex)
            {
                // Log chi tiết lỗi
                Console.WriteLine($"Error in Ask method: {ex.Message}\n{ex.StackTrace}");

                // Trả về mã lỗi 500 với thông báo chi tiết
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }
        }

    }

    public class QueryModel
    {
        public string Query { get; set; }
    }


}
