const Chatconnection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
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
        isAdminMessage: false, 
        content: message
    });
});

Chatconnection.start().then(function () {
    console.log("Connected to SignalR hub");
    Chatconnection.invoke("GetCustomerList").then(function (customers) {
        updateCustomerList(customers);
        updateCustomerTimeAgo(); // Start updating time ago continuously
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
        customerElement.innerHTML = `
            <div class="w-16 h-16 relative flex flex-shrink-0">
                <img class="shadow-md rounded-full w-full h-full object-cover" src="https://randomuser.me/api/portraits/men/97.jpg" alt="" />
                <div class="absolute bg-gray-900 p-1 rounded-full bottom-0 right-0">
                    <div class="bg-green-500 rounded-full w-3 h-3"></div>
                </div>
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
            <div class="bg-blue-700 w-3 h-3 rounded-full flex flex-shrink-0 hidden md:block group-hover:block"></div>
        `;
        customerElement.addEventListener('click', () => loadCustomerMessages(customer.id));
        customerList.appendChild(customerElement);
    });
}

function loadCustomerMessages(customerId) {
    console.log("Invoking GetMessages with customerId:", customerId);
    selectedCustomerId = customerId; // Lưu trữ customerId khi khách hàng được chọn
    Chatconnection.invoke("GetMessages", customerId)
        .then(function (messages) {
            updateMessageList(messages, customerId);
        })
        .catch(function (err) {
            console.error("Error invoking GetMessages:", err.toString());
        });
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

function addMessageToCustomerUI(message) {
    const chatBody = document.querySelector(".chat-body");
    const messageElement = document.createElement("div");

    if (message.isAdminMessage) {
        messageElement.className = "flex flex-row justify-end"; // Tin nhắn của admin
    } else {
        messageElement.className = "flex flex-row justify-start"; // Tin nhắn của khách hàng
    }

    messageElement.innerHTML = `
        <div class="w-8 h-8 relative flex flex-shrink-0 ${message.isAdminMessage ? 'mr-4' : 'ml-4'}">
            <img class="shadow-md rounded-full w-full h-full object-cover" src="https://randomuser.me/api/portraits/${message.isAdminMessage ? 'women/33.jpg' : 'men/97.jpg'}" alt="" />
        </div>
        <div class="messages text-sm text-${message.isAdminMessage ? 'white' : 'gray-700'} grid grid-flow-row gap-2">
            <div class="flex items-center group">
                <p class="px-6 py-3 rounded-t-full rounded-${message.isAdminMessage ? 'l' : 'r'}-full bg-${message.isAdminMessage ? 'blue-700' : 'gray-800 text-gray-200'} max-w-xs lg:max-w-md">
                    ${message.content}
                </p>
                <button type="button" class="option-message">
                    <svg viewBox="0 0 20 20" class="w-full h-full fill-current">
                        <path d="M10.001,7.8C8.786,7.8,7.8,8.785,7.8,10s0.986,2.2,2.201,2.2S12.2,11.215,12.2,10S11.216,7.8,10.001,7.8z
                        M3.001,7.8C1.786,7.8,0.8,8.785,0.8,10s0.986,2.2,2.201,2.2S5.2,11.214,5.2,10S4.216,7.8,3.001,7.8z M17.001,7.8
                        C15.786,7.8,14.8,8.785,14.8,10s0.986,2.2,2.201,2.2S19.2,11.215,19.2,10S18.216,7.8,17.001,7.8z" />
                    </svg>
                </button>
                <button type="button" class="option-message">
                    <svg viewBox="0 0 20 20" class="w-full h-full fill-current">
                        <path d="M19,16.685c0,0-2.225-9.732-11-9.732V2.969L1,9.542l7,6.69v-4.357C12.763,11.874,16.516,12.296,19,16.685z" />
                    </svg>
                </button>
                <button type="button" class="option-message">
                    <svg viewBox="0 0 24 24" class="w-full h-full fill-current">
                        <path d="M12 22a10 10 0 1 1 0-20 10 10 0 0 1 0 20zm0-2a8 8 0 1 0 0-16 8 8 0 0 0 0 16zm-3.54-4.46a1 1 0 0 1 1.42-1.42 3 3 0 0 0 4.24 0 1 1 0 0 1 1.42 1.42 5 5 0 0 1-7.08 0zM9 11a1 1 0 1 1 0-2 1 1 0 0 1 0 2zm6 0a1 1 0 1 1 0-2 1 1 0 0 1 0 2z" />
                    </svg>
                </button>
            </div>
        </div>
    `;

    // Insert messageElement into chatBody
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
    const minutes = Math.floor(diff / 60000);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (minutes < 1) return 'Ngay bây giờ';
    if (minutes < 60) return `${minutes} phút trước`;
    if (hours < 24) return `${hours} h trước`;
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



// Function to send message to customer
function sendMessageToCustomer(customerId, message) {
    if (Chatconnection.state === signalR.HubConnectionState.Connected) {
        Chatconnection.invoke("SendMessageToCustomer", customerId, message)
            .then(() => {
                // Cập nhật UI ngay sau khi gửi tin nhắn thành công
                addMessageToCustomerUI({
                    isAdminMessage: true, // Đánh dấu đây là tin nhắn từ admin
                    content: message
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
