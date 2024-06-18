namespace WebsiteBanHang.Areas.Admin.AdminDTO
{
    public class ResponsExcel<T>
    {
        public int Code { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }

        public static ResponsExcel<T> GetResult(int code, string msg, T data = default(T))
        {
            return new ResponsExcel<T>
            {
                Code = code,
                Msg = msg,
                Data = data
            };
        }
    }
}
