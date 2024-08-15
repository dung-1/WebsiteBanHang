document.getElementById('chatbot-toggle-btn').addEventListener('click', toggleChatbot);
document.getElementById('close-btn').addEventListener('click', toggleChatbot);
document.getElementById('send-btn').addEventListener('click', sendMessage);
document.getElementById('user-input').addEventListener('keypress', function (e) {
    if (e.key === 'Enter') {
        sendMessage();
    }
});

function toggleChatbot() {
    const chatbotPopup = document.getElementById('chatbot-popup');
    chatbotPopup.style.display = chatbotPopup.style.display === 'none' ? 'block' : 'none';
}

function sendMessage() {
    const userInput = document.getElementById('user-input').value.trim();
    if (userInput !== '') {
        appendMessage('user', userInput);
        fetchBotResponse(userInput);
        document.getElementById('user-input').value = '';
    }
}

function fetchBotResponse(userInput) {
    fetch('/api/chat/ask', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ query: userInput })
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    throw new Error(`HTTP error! status: ${response.status}, message: ${text}`);
                });
            }
            return response.json();
        })
        .then(data => {
            const botResponse = data.message;  // Đổi thành 'message' thay vì 'answer'
            appendMessage('bot', botResponse);

            // Nếu bot không hiểu câu hỏi thì thêm lựa chọn 'Yes' hoặc 'No'
            if (botResponse === "I'm sorry, I didn't understand that. Want to connect with expert?") {
                appendYesNoButtons();
            }
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
            appendMessage('bot', 'Error: ' + error.message);
        });
}


function appendMessage(sender, message) {
    const chatBox = document.getElementById('chat-box');
    const messageElement = document.createElement('div');
    messageElement.classList.add(sender === 'user' ? 'user-message' : 'bot-message');
    messageElement.innerHTML = message;
    chatBox.appendChild(messageElement);
    chatBox.scrollTop = chatBox.scrollHeight;
}

function appendYesNoButtons() {
    const chatBox = document.getElementById('chat-box');
    const buttonYes = document.createElement('button');
    buttonYes.textContent = '✔ Yes';
    buttonYes.onclick = function () {
        appendMessage('bot', 'Great! Please wait a moment while we connect you with an expert.');
    };
    const buttonNo = document.createElement('button');
    buttonNo.textContent = '✖ No';
    buttonNo.onclick = function () {
        appendMessage('bot', 'Okay, if you change your mind just let me know!');
    };
    const buttonContainer = document.createElement('div');
    buttonContainer.classList.add('button-container');
    buttonContainer.appendChild(buttonYes);
    buttonContainer.appendChild(buttonNo);
    chatBox.appendChild(buttonContainer);
    chatBox.scrollTop = chatBox.scrollHeight;
}
