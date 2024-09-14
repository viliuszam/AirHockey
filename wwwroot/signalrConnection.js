const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .build();

let roomCode = null;

connection.start().then(function () {
    console.log("Connected to the SignalR hub");
}).catch(function (err) {
    return console.error(err.toString());
});

function createRoom(code) {
    roomCode = code;
    connection.invoke("CreateRoom", roomCode)
        .then(() => {
            console.log(`Room ${roomCode} created`);
        })
        .catch(err => console.error(err.toString()));
}

function joinRoom(code) {
    roomCode = code;
    connection.invoke("JoinRoom", roomCode)
        .then(() => {
            console.log(`Joined room ${roomCode}`);
        })
        .catch(err => console.error(err.toString()));
}

connection.on("RoomCreationFailed", function (message) {
    alert(message);
});

export { connection, createRoom, joinRoom, roomCode };
