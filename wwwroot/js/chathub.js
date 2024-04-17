const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub") // Thay thế bằng URL của Hub của bạn
    .build();

// Xử lý tin nhắn nhận được từ server
connection.on("ReceiveMessage", (message, senderId) => {
    // Hiển thị tin nhắn nhận được trên giao diện chat
    const chatArea = document.getElementById("chat-area");
    const messageElement = document.createElement("div");
    messageElement.classList.add("received-message");
    messageElement.textContent = `From ${senderId}: ${message}`;
    chatArea.appendChild(messageElement);
});

// Kết nối tới Hub SignalR
connection.start().then(() => {
    // Gửi một tin nhắn khi kết nối được thiết lập (tùy chọn)
    // ...
});

// Hàm để gửi tin nhắn từ giao diện chat
function sendMessage(message, recipientId) {
    // Gọi phương thức SendMessage trên Hub và chuyển tin nhắn và ID của người nhận
    connection.invoke("SendMessage", message, recipientId);
}
