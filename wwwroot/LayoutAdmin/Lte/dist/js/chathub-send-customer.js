const Chatconnection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub", {
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets |
            signalR.HttpTransportType.ServerSentEvents |
            signalR.HttpTransportType.LongPolling
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();
Chatconnection.serverTimeoutInMilliseconds = 300000;
let selectedCustomerId = null;
Chatconnection.on("UpdateCustomerList", function (customers) {
    updateCustomerList(customers);
});
Chatconnection.on("ReceiveMessages", function (customerId, message, sentAt) {
    updateCustomerMessage(customerId, message, new Date(sentAt));
    // Cập nhật giao diện chat nếu đang mở
    addMessageToCustomerUI({
        sentAt: new Date(sentAt), // Tạo đối tượng Date từ chuỗi ISO
        isAdminMessage: false,
        content: message
    });
});

Chatconnection.start().then(function () {
    console.log("Connected to SignalR hub");
    Chatconnection.invoke("GetCustomerList").then(function (customers) {
        updateCustomerList(customers);
        updateCustomerTimeAgo();
        updateCustomerTimelastActiveAgo();// Start updating time ago continuously
    }).catch(function (err) {
        console.error("Error invoking GetCustomerList:", err.toString());
    });
}).catch(function (err) {
    console.error("Error starting SignalR connection:", err.toString());
});


function updateCustomerList(customers) {
    const customerList = document.getElementById("customerList");
    customerList.innerHTML = "";

    customers.forEach(customer => {
        const customerElement = document.createElement("div");
        customerElement.className = "flex justify-between items-center p-3 hover:bg-gray-800 rounded-lg relative cursor-pointer";
        customerElement.id = `customer-${customer.id}`;
        let activityStatusHtml = '';
        if (customer.isActive) {
            activityStatusHtml = `
                <div class="absolute bg-gray-900 p-1 rounded-full bottom-0 right-0">
                    <div class="bg-green-500 rounded-full w-3 h-3"></div>
                </div>
            `;
        } else {
            activityStatusHtml = `
                <div class="absolute bg-gray-900 p-1 rounded-full bottom-0 right-0 activity "data-time="${customer.lastActive}">
                  ${timeAgo(new Date(customer.lastActive))}
                </div>
            `;
        }

        customerElement.innerHTML = `
            <div class="w-16 h-16 relative flex flex-shrink-0">
                <img class="shadow-md rounded-full w-full h-full object-cover" src="/img/image_user.png" alt="" />
                 ${activityStatusHtml}
            </div>
            <div class="flex-auto min-w-0 ml-4 mr-6 hidden md:block group-hover:block">
                <p class="font-bold">${customer.name}</p>
                <div class="flex items-center text-sm font-bold">
                    <div class="min-w-0">
                        <p class="truncate">${customer.lastMessage}</p>
                    </div>
                    <p class="ml-2 whitespace-no-wrap" data-time="${customer.lastMessageTimeAgo}">${timeAgo(new Date(customer.lastMessageTimeAgo))}</p>
                </div>
            </div>
        `;
        customerElement.addEventListener('click', () => {
            // Load messages for the selected customer
            loadCustomerMessages(customer.id);

            // Show the header and footer
            document.querySelector(".chat-header").style.display = "block";
            document.querySelector(".chat-footer").style.display = "block";
        });
        // Show the header and footer


        customerList.appendChild(customerElement);
    });
}

function loadCustomerMessages(customerId) {
    console.log("Invoking GetMessages with customerId:", customerId);
    selectedCustomerId = customerId; // Lưu trữ customerId khi khách hàng được chọn
    Chatconnection.invoke("GetMessages", customerId)
        .then(function (messages) {
            updateMessageList(messages, customerId);
            updateActiveNameCustomer(messages);
        })
        .catch(function (err) {
            console.error("Error invoking GetMessages:", err.toString());
        });
}

function updateActiveNameCustomer(messages) {
    const chat_header = document.querySelector(".chat-header");
    chat_header.innerHTML = "";
    if (messages.length > 0) {
        addMessageActiveNameCustomer(messages[0]);
    }
}



function updateMessageList(messages, customerId) {
    const chatBody = document.querySelector(".chat-body");

    // Clear current chat body
    chatBody.innerHTML = "";

    // Append messages to chat body
    messages.forEach(message => {
        addMessageToCustomerUI(message);
    });
}

function addMessageActiveNameCustomer(message) {
    const chatHeader = document.querySelector(".chat-header");
    chatHeader.innerHTML = '';

    const messageElement = document.createElement("div");
    let activityStatusHtml = '';
    let activityStatus = '';
    if (message.isActive) {
        activityStatusHtml = `
            <div class="absolute bg-gray-900 p-1 rounded-full bottom-0 right-0">
                <div class="bg-green-500 rounded-full w-3 h-3"></div>
            </div>
        `;
    }
    if (message.isActive) {
        activityStatus = `
           <div class="text-sm">
                <p class="font-bold">${message.senderName}</p>
                <p>Đang hoạt động</p>
            </div>
        `;
    } else {
        activityStatus = `
         <div class="text-sm activity" data-time="${message.lastActive}">
                <p class="font-bold">${message.senderName}</p>
                     Hoạt động ${timeAgo(new Date(message.lastActive))}
            </div>
        `;
    }
    messageElement.innerHTML = `
        <div class="flex">
            <div class="w-12 h-12 mr-4 relative flex flex-shrink-0">
                <img class="shadow-md rounded-full w-full h-full object-cover" src="/img/image_user.png" alt="" />
                ${activityStatusHtml}
            </div>
           ${activityStatus}
        </div>
    `;
    chatHeader.appendChild(messageElement);
}

let lastMessageDate = null;

function addMessageToCustomerUI(message) {
    const chatBody = document.querySelector(".chat-body");
    const messageElement = document.createElement("div");

    // Định dạng thời gian gửi tin nhắn
    const messageDate = new Date(message.sentAt);
    const formattedDate = messageDate.toLocaleDateString('vi-VN');
    const formattedTime = messageDate.toLocaleTimeString('vi-VN');

    // Tạo mốc thời gian theo ngày nếu ngày hiện tại khác với ngày của tin nhắn trước đó
    if (lastMessageDate !== formattedDate) {
        const dateMarkerElement = document.createElement("p");
        dateMarkerElement.className = "p-4 text-center text-sm text-gray-500";
        dateMarkerElement.innerText = formattedDate;
        chatBody.appendChild(dateMarkerElement);
        lastMessageDate = formattedDate;
    }

    // Tạo phần tử hiển thị tin nhắn
    if (message.isAdminMessage) {
        messageElement.className = "flex flex-row justify-end"; // Tin nhắn của admin
    } else {
        messageElement.className = "flex flex-row justify-start"; // Tin nhắn của khách hàng
    }

    messageElement.innerHTML = `
        <div class="w-8 h-8 relative flex flex-shrink-0 ${message.isAdminMessage ? 'mr-4' : 'ml-4'}">
            ${message.isAdminMessage ? '' : `<img class="shadow-md rounded-full w-full h-full object-cover" src="/img/image_user.png" alt="" />`}
        </div>

        <div class="messages text-sm text-${message.isAdminMessage ? 'white' : 'gray-700'} grid grid-flow-row gap-2">
            <div class="flex items-center group ${message.isAdminMessage ? 'flex-row-reverse' : ''}">
                <p class="px-6 py-3 rounded-t-full rounded-${message.isAdminMessage ? 'l' : 'r'}-full bg-${message.isAdminMessage ? 'blue-700' : 'gray-800 text-gray-200'} max-w-xs lg:max-w-md">
                    ${message.content}
                </p>
            </div>
            <p class="time-messages">${formattedTime}</p>
        </div>
    `;

    // Chèn messageElement vào chatBody
    chatBody.appendChild(messageElement);
}
function updateCustomerMessage(customerId, message, sentAt) {
    selectedCustomerId = customerId;
    const customerElement = document.querySelector(`#customer-${selectedCustomerId}`);
    if (customerElement) {
        const lastMessageElement = customerElement.querySelector('.truncate');
        const timeAgoElement = customerElement.querySelector('.whitespace-no-wrap');
        lastMessageElement.textContent = message;
        timeAgoElement.textContent = timeAgo(new Date(sentAt));
        timeAgoElement.setAttribute('data-time', new Date(sentAt).toISOString());

        // Di chuyển khách hàng lên đầu danh sách
        const customerList = document.getElementById("customerList");
        customerList.insertBefore(customerElement, customerList.firstChild);

    } else {
        // Fetch danh sách khách hàng mới nếu không tìm thấy khách hàng hiện tại
        Chatconnection.invoke("GetCustomerList").catch(function (err) {
            console.error(err.toString());
        });
    }
}
function updateChatInterface(userId, message, sentAt) {
    const chatWindow = document.querySelector('.chat-window[data-user-id="' + userId + '"]');
    if (chatWindow) {
        const chatMessages = chatWindow.querySelector('.chat-messages');
        const messageElement = document.createElement('div');
        messageElement.className = 'message customer-message';
        messageElement.innerHTML = `
            <p>${message}</p>
            <span class="message-time">${formatTime(new Date(sentAt))}</span>
        `;
        chatMessages.appendChild(messageElement);
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }
}
function formatTime(date) {
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' });
}
function timeAgo(date) {
    const now = new Date();
    const diff = now - date;
    const seconds = Math.floor(diff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (seconds < 60) return `${seconds} giây trước`;
    if (minutes < 60) return `${minutes} phút trước`;
    if (hours < 24) return `${hours} giờ trước`;
    return `${days} ngày trước`;
}
function updateCustomerTimeAgo() {
    setInterval(() => {
        const customerElements = document.querySelectorAll('[id^=customer-]');
        customerElements.forEach(customerElement => {
            const timeAgoElement = customerElement.querySelector('.whitespace-no-wrap');
            const lastMessageTime = new Date(timeAgoElement.getAttribute('data-time'));
            if (!isNaN(lastMessageTime)) {
                timeAgoElement.textContent = timeAgo(lastMessageTime);
            }
        });
    }, 60000); // Update every minute
}
function updateCustomerTimelastActiveAgo() {
    setInterval(() => {
        const customerElements = document.querySelectorAll('[id^=customer-]');
        customerElements.forEach(customerElement => {
            const timeAgoElement = customerElement.querySelector('.activity');
            const lastMessageTime = new Date(timeAgoElement.getAttribute('data-time'));
            if (!isNaN(lastMessageTime)) {
                timeAgoElement.textContent = timeAgo(lastMessageTime);
            }
        });
    }, 60000); // Update every minute
}
function sendMessageToCustomer(customerId, message) {
    const sentAt = new Date().toISOString(); // Lấy thời gian hiện tại và định dạng thành chuỗi ISO

    if (Chatconnection.state === signalR.HubConnectionState.Connected) {
        Chatconnection.invoke("SendMessageToCustomer", customerId, message)
            .then(() => {
                // Gọi server để cập nhật danh sách khách hàng mới nhất
                Chatconnection.invoke("GetCustomerList").then(function (customers) {
                    updateCustomerList(customers);
                }).catch(function (err) {
                    console.error("Error invoking GetCustomerList:", err.toString());
                });

                // Cập nhật UI ngay sau khi gửi tin nhắn thành công
                addMessageToCustomerUI({
                    isAdminMessage: true, // Đánh dấu đây là tin nhắn từ admin
                    content: message,
                    sentAt: new Date(sentAt), // Tạo đối tượng Date từ chuỗi ISO

                });
            })
            .catch(err => console.error("Error sending message:", err.toString()));
    } else {
        console.log("Connection is not established. Message not sent.");
    }
}

document.addEventListener('DOMContentLoaded', function () {
    // Event listener for send button click
    const sendButton = document.getElementById("sendButton");
    const messageInput = document.getElementById("messageInput");

    if (sendButton && messageInput) {
        sendButton.addEventListener("click", event => {
            const message = messageInput.value;

            if (!message) {
                console.error("Message is empty.");
                return;
            }

            if (!selectedCustomerId) {
                console.error("No customer selected.");
                return;
            }

            console.log("Sending message to customer with ID:", selectedCustomerId);

            sendMessageToCustomer(selectedCustomerId, message); // Truyền selectedCustomerId vào hàm gửi tin nhắn
            messageInput.value = '';
        });
    } else {
        console.error("Send button or message input not found.");
    }
});
