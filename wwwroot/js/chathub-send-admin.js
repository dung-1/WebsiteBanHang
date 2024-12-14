const Customerconnection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub", {
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets |
            signalR.HttpTransportType.ServerSentEvents |
            signalR.HttpTransportType.LongPolling 
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();
Customerconnection.serverTimeoutInMilliseconds = 300000;
// Start the connection initially
Customerconnection.start().then(function () {
    console.log("Connected to SignalR hub");
    const userId = parseInt(document.querySelector('.chat-window2').getAttribute('data-user-id'), 10);
    Customerconnection.invoke("GetChatHistory", userId).then(function (messages) {
        updateMessageList(messages);
    }).catch(function (err) {
        console.error("Error invoking GetChatHistory:", err.toString());
    });
}).catch(function (err) {
    console.error("Error starting SignalR connection:", err.toString());
});

// Handle connection closure
Customerconnection.onclose(() => {
    console.error("Connection closed. Attempting to reconnect...");
});

document.getElementById("sendButton").addEventListener("click", event => {
    const message = document.getElementById("messageInput").value;

    if (!message) {
        console.error("Message is empty.");
        return;
    }

    sendMessageToAdmin(message);
    document.getElementById("messageInput").value = '';
});

function sendMessageToAdmin(message) {
    const sentAt = new Date().toISOString(); // Lấy thời gian hiện tại và định dạng thành chuỗi ISO

    if (Customerconnection.state === signalR.HubConnectionState.Connected) {
        Customerconnection.invoke("SendMessageToAdmin", message)
            .then(() => {
                addMessageToAdminUI({
                    senderId: Customerconnection.connectionId,
                    message: message,
                    sentAt: new Date(sentAt), // Tạo đối tượng Date từ chuỗi ISO
                    isFromAdmin: false
                });
            })
            .catch(err => console.error("Error sending message:", err.toString()));
    } else {
        console.log("Connection is not established. Message not sent.");
    }
}



Customerconnection.on("ReceiveMessagetoCustomer", function (senderConnectionId, message, sentAt) {
    addMessageToAdminUI({
        senderId: senderConnectionId,
        message: message,
        sentAt: new Date(sentAt), // Tạo đối tượng Date từ chuỗi ISO
        isFromAdmin: senderConnectionId !== Customerconnection.connectionId
    });
});


function formatMessage(message) {
    return `${message.message}`;
}

let lastMessageDateAdmin = null;

function addMessageToAdminUI(message) {
    const messageBox = document.getElementById("messageBox");
    const messageDate = new Date(message.sentAt);
    const formattedDate = messageDate.toLocaleDateString('vi-VN');
    const formattedTime = messageDate.toLocaleTimeString('vi-VN');

    // Tạo mốc thời gian theo ngày nếu ngày hiện tại khác với ngày của tin nhắn trước đó
    if (lastMessageDateAdmin !== formattedDate) {
        const dateMarkerElement = document.createElement("p");
        dateMarkerElement.className = "p-4 text-center text-sm text-gray-500";
        dateMarkerElement.innerText = formattedDate;
        messageBox.appendChild(dateMarkerElement);
        lastMessageDateAdmin = formattedDate;
    }

    // Tạo phần tử hiển thị tin nhắn
    const div = document.createElement("div");

    if (message.isFromAdmin) {
        div.className = "second-chat";
        div.innerHTML = `
            <div class="circle"></div>
            <p class="conten-message-customer">${message.message}</p>
            <p class="time-messages">${formattedTime}</p>
            <div class="arrow"></div>
        `;
    } else {
        div.className = "first-chat";
        div.innerHTML = `
            <p class="conten-message-admin">${message.message}</p>
            <p class="time-messages">${formattedTime}</p>
        `;
    }

    // Chèn div vào messageBox
    messageBox.appendChild(div);
}


function updateMessageList(messages) {
    const messageBox = document.getElementById("messageBox");
    messageBox.innerHTML = ""; // Clear existing messages

    for (const message of messages) {
        addMessageToAdminUI(message);
    }
}
