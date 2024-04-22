let connection = new signalR.HubConnectionBuilder().withUrl("/hubs/chat").build();

connection.start().then(() => {
    console.log('Presence connection started');
}).catch(err => console.error('Error while starting connection: ' + err));

connection.on('UserConnected', (connId, userId) => {
    console.log('User connected', connId, userId);
});

connection.on('UserDisconnected', (connId) => {
    console.log('User disconnected', connId);
});

connection.on('ReceiveMessage', messageId => {
    $.ajax({
        url: `/Chats/GetMessage/${messageId}`,
        type: 'GET',
        success: data => {
            console.log(data);
            if (window.location.pathname.toLowerCase().startsWith('/chats/chat')) {
                console.log(data);
                let dateTime = new Date(data.createdAt);
                let formattedDate = formatDate(dateTime);
                let div = createMessageElement(data.id, data.sender.userName, formattedDate, data.isRead, data.text, data.imageUrl);
                console.log(div);
                $('.chat-container').append(div);
            }
        },
        error: err => console.error(err)
    });
    connection.invoke('MarkAsRead', messageId).catch(err => console.error(err));
});

connection.on('MessageRead', messageId => {
    console.log('Message read', messageId);
    let message = $(`.chat-message[data-id=${messageId}]`);
    let check = message.find('.bi-check-all');
    check.css('color', '#a52834');
});

function formatDate(dateTime) {
    let month = dateTime.getMonth() + 1;
    let day = dateTime.getDate();
    let minutes = dateTime.getMinutes();
    let seconds = dateTime.getSeconds();
    let hours = dateTime.getHours();
    let formattedDate = '';
    if (day < 10) formattedDate += '0';
    formattedDate += day + '.';
    if (month < 10) formattedDate += '0';
    formattedDate += month + '.';
    formattedDate += dateTime.getFullYear() + ' ';
    if (hours < 10) formattedDate += '0';
    formattedDate += hours + ':';
    if (minutes < 10) formattedDate += '0';
    formattedDate += minutes + ':';
    if (seconds < 10) formattedDate += '0';
    formattedDate += seconds;
    return formattedDate;
}

function createMessageElement(id, from, time, isRead, messageText, imageUrl, darker = false) {
    let div = $('<div class="chat-message"></div>').attr('data-id', id);
    if (darker) {
        div.addClass('darker');
    }
    let messageHeader = $('<div class="chat-message-header"></div>');
    let senderName = $('<span class="chat-message-sender"></span>').text(from);
    let messageDate = $('<span class="chat-message-date"></span>').text(time);
    let readCheck;
    if (isRead) {
        readCheck = $('<i class="bi bi-check-all" style="color: #a52834"></i>');
    } else {
        readCheck = $('<i class="bi bi-check-all" style="color: #41464b"></i>');
    }
    messageHeader.append(senderName);
    messageHeader.append(messageDate);
    messageHeader.append(readCheck);
    let messageContent = $('<div class="chat-message-content"></div>');
    if (imageUrl) {
        console.log(imageUrl)
        if (!imageUrl.startsWith('/')) {
            imageUrl = '/' + imageUrl;
        }
        console.log(imageUrl)
        imageUrl = imageUrl.replace('\\', '/');
        console.log(imageUrl)
        let image = $('<img class="message-image" />').attr('src', imageUrl);
        messageContent.append(image);
    }
    if (messageText) {
        let text = $('<p class="chat-message-text"></p>').text(messageText);
        messageContent.append(text);
    }
    div.append(messageHeader);
    div.append(messageContent);
    return div;
}