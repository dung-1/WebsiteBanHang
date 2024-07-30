using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Common
{
    public static class EnumHelper
    {
        public static string GetDescription<T>(this T enumValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DisplayAttribute)attrs[0]).Name;
                }
            }

            return description;
        }
    }
}
