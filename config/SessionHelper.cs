namespace WebsiteBanHang.config
{
public class SessionHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetSession(string key, string value)
    {
        _httpContextAccessor.HttpContext.Session.SetString(key, value);
    }

    public string GetSession(string key)
    {
        return _httpContextAccessor.HttpContext.Session.GetString(key);
    }
}

}
