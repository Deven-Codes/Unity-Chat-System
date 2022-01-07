const WebSocket = require('ws');

const PORT = 5050;

const wsServer = new WebSocket.Server({
    port: PORT
});

wsServer.on('connection', (socket) => {
    // Some feedback on the console
    console.log("A client just connected");

    // Attach some behavior to the incoming socket
    socket.on('message', (data) => {
        
        console.log('Received Data:\n %o', data.toString());

        // Broadcast that message to all connected clients
        wsServer.clients.forEach((client)=> {   

            console.log(data);
            client.send(data);

        });

    });

    socket.on('close', function () {
        console.log('Client disconnected');
    })

});

console.log("WebSocket Server is listening on port " + PORT);