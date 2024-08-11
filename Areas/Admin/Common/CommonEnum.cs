using System.ComponentModel.DataAnnotations;

namespace WebsiteBanHang.Areas.Admin.Common
{
    public enum StatusActivity
    {
        [Display(Name = "Kích hoạt")]
        Active = 1,
        [Display(Name = "Kích hoạt nội bộ")]
        ActiveInternal = 2,
        [Display(Name = "Khóa")]
        InActive = 3
    }

    public enum CancelOfAdmin
    {
        [Display(Name = "Sản phẩm hết hàng")]
        CancelReson_1 = 1,

        [Display(Name = "Thông tin đơn hàng không chính xác")]
        CancelReson_2 = 2,

        [Display(Name = "Khách hàng yêu cầu hủy")]
        CancelReson_3 = 3,

        [Display(Name = "Khách hàng không phản hồi")]
        CancelReson_4 = 4,

        [Display(Name = "Lỗi hệ thống")]
        CancelReson_5 = 5,

        [Display(Name = "Phát hiện dấu hiệu gian lận")]
        CancelReson_6 = 6,

        [Display(Name = "Thanh toán không thành công")]
        CancelReson_7 = 7
    }

    public enum CancelOfClient
    {
        [Display(Name = "Thay đổi ý định mua hàng")]
        CancelReson_1 = 1,

        [Display(Name = "Tìm thấy giá tốt hơn")]
        CancelReson_2 = 2,

        [Display(Name = "Thời gian giao hàng quá lâu")]
        CancelReson_3 = 3,

        [Display(Name = "Thông tin sản phẩm không chính xác")]
        CancelReson_4 = 4,

        [Display(Name = "Sai sót khi đặt hàng")]
        CancelReson_5 = 5,

        [Display(Name = "Vấn đề tài chính")]
        CancelReson_6 = 6,

        [Display(Name = "Đã mua sản phẩm khác")]
        CancelReson_7 = 7
    }


}
