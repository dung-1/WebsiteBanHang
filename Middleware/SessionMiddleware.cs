namespace WebsiteBanHang.Middleware
{
    public class SessionMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Thiết lập giá trị session mặc định, ví dụ:
            if (!context.Session.Keys.Contains("UserSession"))
            {
                // Thiết lập session tại đây
                context.Session.SetString("UserSession", "SomeDefaultValue");
            }

            // Gọi middleware tiếp theo
            await _next(context);
        }
    }

}
