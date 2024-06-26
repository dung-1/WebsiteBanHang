const Chatconnection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();



Chatconnection.on("UpdateCustomerList", function (customers) {
    const customerList = document.getElementById("customerList");
    customerList.innerHTML = "";

    customers.forEach(customer => {
        const customerElement = document.createElement("div");
        customerElement.className = "flex justify-between items-center p-3 hover:bg-gray-800 rounded-lg relative";
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
                    <p class="ml-2 whitespace-no-wrap">${timeAgo(new Date(customer.lastMessageTime))}</p>
                </div>
            </div>
            <div class="bg-blue-700 w-3 h-3 rounded-full flex flex-shrink-0 hidden md:block group-hover:block"></div>
        `;
        customerList.appendChild(customerElement);
    });
});

function timeAgo(date) {
    const now = new Date();
    const diff = now - date;
    const minutes = Math.floor(diff / 60000);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (minutes < 1) return 'just now';
    if (minutes < 60) return `${minutes} min`;
    if (hours < 24) return `${hours} h`;
    return `${days} d`;
}

Chatconnection.start().then(function () {
    connection.invoke("GetCustomerList").catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});
