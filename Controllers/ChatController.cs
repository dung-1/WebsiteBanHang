namespace WebsiteBanHang.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Linq;
    using WebsiteBanHang.Service;

    [ApiController]
    [Route("api/chat")]
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
                return BadRequest("Query is required and cannot be empty.");
            }

            try
            {
                var response = _dialogflowService.DetectIntent("unique-session-id", queryModel.Query);
                return Ok(new { message = response });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in Ask method: {ex}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


    }
    public class QueryModel
{
    public string Query { get; set; }
}

}
