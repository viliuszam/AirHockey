import { connection, createRoom, joinRoom, roomCode } from './signalrConnection.js';
import { drawGame, updatePlayerPosition, updatePuckPosition, player1, player2 } from './canvasRenderer.js';

const roomCodeInput = document.getElementById('roomCode');
const createRoomBtn = document.getElementById('createRoomBtn');
const joinRoomBtn = document.getElementById('joinRoomBtn');
const gameContainer = document.getElementById('game-container');
const roomSelectionContainer = document.getElementById('room-selection');

let playerId;
let keys = { w: false, a: false, s: false, d: false };
let isMoving = false;

createRoomBtn.addEventListener('click', function () {
    const roomCode = roomCodeInput.value;
    const nickname = document.getElementById('nickname').value;
    if (roomCode && nickname) {
        createRoom(roomCode, nickname);
        startGameUI();
    } else {
        alert("Please enter a room code and a nickname.");
    }
});

joinRoomBtn.addEventListener('click', function () {
    const roomCode = roomCodeInput.value;
    const nickname = document.getElementById('nickname').value;
    if (roomCode && nickname) {
        joinRoom(roomCode, nickname);
        startGameUI();
    } else {
        alert("Please enter a room code and a nickname.");
    }
});

function startGameUI() {
    roomSelectionContainer.style.display = 'none';
    gameContainer.style.display = 'block';
}

connection.on("AssignPlayer", function (player) {
    playerId = player;
    console.log(`You are ${player}`);
});

function resetGameUI() {
    roomSelectionContainer.style.display = 'block';
    gameContainer.style.display = 'none';
    console.log("UI reset");
}

connection.on("RoomNotFound", function (message) {
    alert(message);
    resetGameUI();
});

connection.on("StartGame", function (player1Nickname, player2Nickname) {
    //TODO: perdaryt vardu perdavima
    player1.nickname = player1Nickname;
    player2.nickname = player2Nickname;
    console.log("Game is starting!");
    startGame();
});

connection.on("WaitingForPlayer", function () {
    alert("Waiting for another player to join...");
});

connection.on("RoomFull", function (message) {
    alert(message);
});

connection.on("UpdateGameState", function (player1X, player1Y, player2X, player2Y, puckX, puckY) {
    updatePlayerPosition("Player1", player1X, player1Y);
    updatePlayerPosition("Player2", player2X, player2Y);
    updatePuckPosition(puckX, puckY);

    drawGame(roomCode);

    console.log(`Received game state: Player1 (${player1X}, ${player1Y}), Player2 (${player2X}, ${player2Y}), Puck (${puckX}, ${puckY})`);
});

connection.on("PlayerDisconnected", function (message) {
    alert(message);
    resetGameUI();
});

function startGame() {
    drawGame(roomCode);
    startInputLoop();
}

document.addEventListener('keydown', function (event) {
    if (event.key === 'w') keys.w = true;
    if (event.key === 'a') keys.a = true;
    if (event.key === 's') keys.s = true;
    if (event.key === 'd') keys.d = true;
    isMoving = true;
});

document.addEventListener('keyup', function (event) {
    if (event.key === 'w') keys.w = false;
    if (event.key === 'a') keys.a = false;
    if (event.key === 's') keys.s = false;
    if (event.key === 'd') keys.d = false;
    isMoving = keys.w || keys.a || keys.s || keys.d;
});

function sendInputState() {
    if (roomCode && playerId) {
        connection.invoke("UpdateInput", roomCode, playerId, keys.w, keys.s, keys.a, keys.d)
            .catch(function (err) {
                console.error("Error sending input state:", err.toString());
            });
    }
}

function startInputLoop() {
    setInterval(() => {
        if (isMoving) {
            sendInputState();
        }
    }, 16); // 60 kartu per sekunde siuncia inputus
}