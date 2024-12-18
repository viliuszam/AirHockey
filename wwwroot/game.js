import { connection, createRoom, joinRoom, roomCode } from './signalrConnection.js';
import { drawGame, updatePlayerPosition, updatePuckPosition, player1, player2, addWall, addAchievement, updateWallPosition, clearWalls, addPowerup, updateEffects, updatePowerups, updatePlayerActivePowerups, addLightingEffect } from './canvasRenderer.js';

const roomCodeInput = document.getElementById('roomCode');
const createRoomBtn = document.getElementById('createRoomBtn');
const joinRoomBtn = document.getElementById('joinRoomBtn');
const gameContainer = document.getElementById('game-container');
const roomSelectionContainer = document.getElementById('room-selection');

let playerId;
let keys = { w: false, a: false, s: false, d: false, e: false };
let isMoving = false;
let consoleActive = false;
let isGamePaused = false;
let pausedPlayer;

export let scoreMessage = null;
let scoreMessageTimeout = null;
let isGameActive = false;

const soundEffects = {
    1: new Audio('/sounds/scored.mp3'),
    2: new Audio('/sounds/powerup.mp3'),
    3: new Audio('/sounds/collision.mp3'),
    4: new Audio('/sounds/gamestart.mp3')
};

function playSoundEffect(effectName) {
    const sound = soundEffects[effectName];
    if (sound) {
        sound.currentTime = 0; // Reset playback to the beginning
        sound.play().catch((error) => {
            console.error(`Error playing sound "${effectName}":`, error);
        });
        console.log("played it fr");
    } else {
        console.warn(`Sound effect "${effectName}" not found.`);
    }
}

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
    isGameActive = false;
    console.log("UI reset");
}

connection.on("RoomNotFound", function (message) {
    alert(message);
    resetGameUI();
});

connection.on("UpdateScores", function (player1Score, player2Score) {
    player1.score = player1Score;
    player2.score = player2Score;

    drawGame(roomCode);
});

connection.on("StartGame", function (player1Nickname, player2Nickname, player1Score, player2Score) {
    //TODO: perdaryt vardu perdavima
    player1.nickname = player1Nickname;
    player2.nickname = player2Nickname;
    player1.score = player1Score;
    player2.score = player2Score;
    console.log("Game is starting!");
    startGame();
});

connection.on("Achievement", function (message) {
    console.log(message);
    addAchievement(message);
} );

connection.on("WaitingForPlayer", function () {
    alert("Waiting for another player to join...");
});

connection.on("RoomFull", function (message) {
    alert(message);
});

connection.on("ShowLightingEffect", function (x, y, width, height, duration, color) {
    addLightingEffect(x, y, width, height, duration, color);
});
connection.on("PlaySoundEffect", function (type, volume) {
    console.log("sound: " + type + " " + volume);
    playSoundEffect(type);
});

connection.on("UpdateGameState", function (player1X, player1Y, player2X, player2Y, puckX, puckY, sentWalls, activeEffects, activePowerups, playerActivePowerups) {
    updatePlayerPosition("Player1", player1X, player1Y);
    updatePlayerPosition("Player2", player2X, player2Y);
    updatePuckPosition(puckX, puckY);
    updateWallPosition(sentWalls);
    updateEffects(activeEffects);
    updatePowerups(activePowerups);
    updatePlayerActivePowerups(playerActivePowerups);

    drawGame(roomCode);
    //console.log("effects: " + activeEffects);
    //activeEffects.forEach(eff => {
    //    console.log(`effect: ${eff.EffectType} ${eff.Duration} ${eff.Behavior} ${eff.X} ${eff.Y} ${eff.Radius}`);
    //});
    //sentWalls.forEach(wall => {
    //    console.log(`Wall Id: ${wall.id}, X: ${wall.x}, Y: ${wall.y}, Width: ${wall.width}, Height: ${wall.height}, Type: ${wall.name}`);
    //});
});
connection.on("PlayerWon", function (winnerNickname, score) {
    alert(`${winnerNickname} wins the game with a score of ${score}!`);

    showWinnerMessage(winnerNickname, score);

    clearWalls();
    clearPowerups();
    resetGameUI();
});
connection.on("PlayerDisconnected", function (message) {
    alert(message);
    this.playerId = null;
    this.roomCode = null;
    isGameActive = false;
    clearWalls();
    clearPowerups();
    resetGameUI();
});

function startGame() {
    isGameActive = true;
    drawGame(roomCode);
    startInputLoop();
}

document.addEventListener('keydown', function (event) {
    if (consoleActive) return;
    if (event.key === 'w') keys.w = true;
    if (event.key === 'a') keys.a = true;
    if (event.key === 's') keys.s = true;
    if (event.key === 'd') keys.d = true;
    if (event.key === 'e') keys.e = true;
    if (event.key === 'p') pauseGame();
    isMoving = keys.w || keys.a || keys.s || keys.d;
});

document.addEventListener('keyup', function (event) {
    if (consoleActive) return;
    if (event.key === 'w') keys.w = false;
    if (event.key === 'a') keys.a = false;
    if (event.key === 's') keys.s = false;
    if (event.key === 'd') keys.d = false;
    if (event.key === 'e') keys.e = false;
    isMoving = keys.w || keys.a || keys.s || keys.d;
});

connection.on("GoalScored", function (scoringPlayer, player1Score, player2Score) {
    scoreMessage = `${scoringPlayer} scores!`;
    clearTimeout(scoreMessageTimeout);
    scoreMessageTimeout = setTimeout(() => scoreMessage = null, 2000);

    player1.score = player1Score;
    player2.score = player2Score;

    drawGame(roomCode);
});

connection.on("AddWall", function (wallId, x, y, width, height, wallType) {
    addWall(wallId, x, y, width, height, wallType); 
    drawGame(roomCode); 
});

connection.on("AddPowerup", function (powerupId, x, y, powerupType, isActive) {
    addPowerup(powerupId, x, y, powerupType, isActive);
    drawGame(roomCode);
});

function sendInputState() {
    if (isGameActive && roomCode != null && playerId != null) {
        //console.log("Sending input:" + roomCode + " " + playerId + " " + keys.w + " " + keys.s + " " + keys.a + " " + keys.d + " " + keys.e);
        connection.invoke("UpdateInput", roomCode, playerId, keys.w, keys.s, keys.a, keys.d, keys.e, false)
            .catch(function (err) {
                console.error("Error sending input state:", err.toString());
            });
    }
}

function pauseGame() {
    if(!isGamePaused || (isGamePaused && pausedPlayer === playerId)){
        connection.invoke("UpdateInput", roomCode, playerId, keys.w, keys.s, keys.a, keys.d, keys.e, true)
            .catch(function (err) {
                console.error("Error sending input state:", err.toString());
            });
    }
}

function startInputLoop() {
    isGameActive = true;
    setInterval(() => {
        if (isGameActive) {
            sendInputState();
        }
    }, 16); // 60 kartu per sekunde siuncia inputus
}

connection.on("TogglePause", function (pausedId, pausedNickname) {
    isGamePaused = !isGamePaused;

    const pauseOverlayId = "pauseOverlay";
    let pauseOverlay = document.getElementById(pauseOverlayId);

    if (isGamePaused) {
        console.log("Game Paused");
        pausedPlayer = pausedId
        if (!pauseOverlay) {
            pauseOverlay = document.createElement("div");
            pauseOverlay.id = pauseOverlayId;
            pauseOverlay.style.position = "fixed";
            pauseOverlay.style.top = "0";
            pauseOverlay.style.left = "0";
            pauseOverlay.style.width = "100%";
            pauseOverlay.style.height = "100%";
            pauseOverlay.style.backgroundColor = "rgba(0, 0, 0, 0.5)"; // Semi-transparent gray
            pauseOverlay.style.display = "flex";
            pauseOverlay.style.alignItems = "center";
            pauseOverlay.style.justifyContent = "center";
            pauseOverlay.style.zIndex = "1000"; // Ensure it's above all other elements

            const pauseBox = document.createElement("div");
            pauseBox.style.backgroundColor = "white";
            pauseBox.style.padding = "20px";
            pauseBox.style.borderRadius = "10px";
            pauseBox.style.boxShadow = "0 4px 8px rgba(0, 0, 0, 0.2)";
            pauseBox.style.textAlign = "center";

            const pauseText = document.createElement("p");
            pauseText.style.margin = "0";
            pauseText.style.fontSize = "20px";
            pauseText.style.color = "black";
            pauseText.textContent = `Game is Paused by ${pausedNickname || "Unknown Player"}`;

            pauseBox.appendChild(pauseText);
            pauseOverlay.appendChild(pauseBox);
            document.body.appendChild(pauseOverlay);
        }

    } else {
        console.log("Game Resumed");

        // Remove overlay if it exists
        if (pauseOverlay) {
            pauseOverlay.remove();
        }
    }
});
function showWinnerMessage(winnerNickname, score) {
    const winnerOverlayId = "winnerOverlay";
    let winnerOverlay = document.getElementById(winnerOverlayId);

    if (!winnerOverlay) {
        winnerOverlay = document.createElement("div");
        winnerOverlay.id = winnerOverlayId;
        winnerOverlay.style.position = "fixed";
        winnerOverlay.style.top = "0";
        winnerOverlay.style.left = "0";
        winnerOverlay.style.width = "100%";
        winnerOverlay.style.height = "100%";
        winnerOverlay.style.backgroundColor = "rgba(0, 0, 0, 0.7)"; // Semi-transparent dark background
        winnerOverlay.style.display = "flex";
        winnerOverlay.style.alignItems = "center";
        winnerOverlay.style.justifyContent = "center";
        winnerOverlay.style.zIndex = "1000"; // Ensure it's above all other elements

        const winnerBox = document.createElement("div");
        winnerBox.style.backgroundColor = "white";
        winnerBox.style.padding = "20px";
        winnerBox.style.borderRadius = "10px";
        winnerBox.style.boxShadow = "0 4px 8px rgba(0, 0, 0, 0.2)";
        winnerBox.style.textAlign = "center";

        const winnerText = document.createElement("p");
        winnerText.style.margin = "0";
        winnerText.style.fontSize = "24px";
        winnerText.style.color = "black";
        winnerText.textContent = `${winnerNickname} wins the game with a score of ${score}`;

        winnerBox.appendChild(winnerText);
        winnerOverlay.appendChild(winnerBox);
        document.body.appendChild(winnerOverlay);
    }
}
function toggleConsole() {
    const consoleInput = document.getElementById("consoleInput");

    if (!consoleInput) {
        const input = document.createElement("input");
        input.id = "consoleInput";
        input.type = "text";
        input.placeholder = "Enter command...";
        input.style.position = "absolute";
        input.style.bottom = "10px";
        input.style.left = "10px";
        input.style.width = "calc(100% - 20px)";
        input.style.padding = "10px";
        input.style.fontSize = "16px";
        input.style.zIndex = "1000";
        document.body.appendChild(input);
        
        input.addEventListener("keydown", (e) => {
            if (e.key === "Enter") {
                processCommand(input.value);
                input.value = "";
            } 
        });

        input.focus();
    } else {
        consoleInput.remove();
    }

    consoleActive = !consoleActive;
}

// Function to process console commands
function processCommand(command) {
    connection.invoke("ProcessCommand", roomCode, playerId, command)
        .catch(function (err) {
            console.error("Error sending input state:", err.toString());
        });
}

document.addEventListener("keydown", (e) => {
    if (e.key === "`") { 
        e.preventDefault();
        toggleConsole();
    }
});