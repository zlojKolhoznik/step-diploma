let connection = new signalR.HubConnectionBuilder().withUrl("/hubs/chat").build();
connection.start().then(() => {
    console.log('Connection started');
    $('#sender').val(connection.connectionId);
}).catch(err => console.error('Error while starting connection: ' + err));
$('#send').on('click', () => {
    console.log('Send button clicked')
    let message = $('#message').val();
    let senderName = $('#senderName').val();
    connection.invoke('SendMessage', $('#receiver').val(), message, senderName);
    $('#messages').append(`<li><b>${senderName}</b>: ${message}</li>`);
    $('#message').val('');
});

connection.on('ReceiveMessage', (message, user) => {
    console.log('Message received');
    $('#messages').append(`<li><b>${user}</b>: ${message}</li>`);
});

