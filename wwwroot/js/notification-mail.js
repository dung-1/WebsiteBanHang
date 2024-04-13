// Khởi tạo kết nối SignalR
var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

// Xử lý sự kiện khi nhận được thông báo về đơn hàng mới
connection.on("ReceiveOrderNotification", function (orderCode) {
    // Hiển thị thông báo về đơn hàng mới
    $.ajax({
        type: "GET",
        url: '/User/Index',
        success: function (result) {
            const Toast = Swal.mixin({
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 2500,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.onmouseenter = Swal.stopTimer;
                    toast.onmouseleave = Swal.resumeTimer;
                }
            });
            Toast.fire({
                icon: "success",
                title: "Đơn hàng vừa được duyệt hãy kiểm tra Email"
            })
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });

});


// Bắt đầu kết nối SignalR
connection.start().then(function () {
    console.log("SignalR connection started.");
}).catch(function (err) {
    console.error("SignalR connection error: ", err.toString());
});

// Xử lý sự kiện khi nhấn nút "Mua Hàng"
$(".create-checkout").click(function () {
    // Gửi thông điệp SignalR để thông báo về việc đặt hàng mới
    connection.invoke("SendOrderNotification", "order123")
        .catch(function (err) {
            console.error("Error invoking SendOrderNotification: ", err.toString());
        });
});

// Hàm load lại trang bằng AJAX
