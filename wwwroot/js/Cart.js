function addToCart(id) {
    $.ajax({
        type: "POST",
        url: "/Cart/AddToCart/" + id,
        success: function (response) {
            // Xử lý kết quả trả về nếu cần thiết
            // Ví dụ: cập nhật giao diện người dùng

            // Chuyển đến trang giỏ hàng
            window.location.href = "/Cart/Index";
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi nếu có
        }
    });
}
