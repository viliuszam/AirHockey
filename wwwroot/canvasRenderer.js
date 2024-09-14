import { Player } from './player.js';
import { Puck } from './puck.js';

let canvas = document.getElementById("gameCanvas");
let ctx = canvas.getContext("2d");

export let player1 = new Player("Player1", "red", 100, 200);
export let player2 = new Player("Player2", "blue", 700, 200);
let puck = new Puck(400, 270);

const tableImage = new Image();
tableImage.src = "table.png";

let tableImageLoaded = false;
tableImage.onload = () => {
    tableImageLoaded = true;
};

function drawGame(roomCode) {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    if (tableImageLoaded) {
        ctx.drawImage(tableImage, 0, 0, 855, 541);
    }

    player1.render(ctx);
    player2.render(ctx);
    puck.render(ctx);

    ctx.font = "20px Arial";
    ctx.fillStyle = "black";
    ctx.textAlign = "center";
    ctx.fillText(`Room: ${roomCode}`, canvas.width / 2, canvas.height - 10);
}

function updatePlayerPosition(playerId, x, y) {
    if (playerId === "Player1") {
        player1.move(x, y);
    } else if (playerId === "Player2") {
        player2.move(x, y);
    }
}

function updatePuckPosition(x, y) {
    puck.move(x, y);
}

export { drawGame, updatePlayerPosition, updatePuckPosition };
