﻿// Khởi tạo kết nối SignalR
var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub", {
    skipNegotiation: false,
    transport: signalR.HttpTransportType.WebSockets |
        signalR.HttpTransportType.ServerSentEvents |
        signalR.HttpTransportType.LongPolling
})
    .build();

// Xử lý sự kiện khi nhận được thông báo về đơn hàng mới
connection.on("ReceiveOrderNotification", function (orderCode) {
    // Hiển thị thông báo về đơn hàng mới
    $.ajax({
        type: "GET",
        url: '/admin/Billorder/Index',
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
                title: "Bạn vừa nhận 1 đơn hàng"
            }).then(function () {
                location.reload();
            }).catch(function (err) {
                console.error("SignalR connection error: ", err.toString());
            });

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