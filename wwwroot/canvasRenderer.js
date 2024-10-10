import { Player } from './player.js';
import { Puck } from './puck.js';
import { scoreMessage } from './game.js';
import { Wall } from './wall.js';

let canvas = document.getElementById("gameCanvas");
let ctx = canvas.getContext("2d");

export let player1 = new Player("Player1", "red", 100, 200);
export let player2 = new Player("Player2", "blue", 700, 200);
let puck = new Puck(400, 270);
let walls = [];

const tableImage = new Image();
tableImage.src = "table.png";

let tableImageLoaded = false;
tableImage.onload = () => {
    tableImageLoaded = true;
};

function addWall(wallId, x, y, width, height, wallType) {
    walls.push(new Wall(wallId, x, y, width, height, wallType));
}

export function clearWalls() {
    walls = [];
}

function updateWallPosition(sentWalls) {
    sentWalls.forEach(sentWall => {
        let matchingWall = walls.find(wall => wall.wallId == sentWall.id); 
        if (matchingWall) {
            matchingWall.move(sentWall.x, sentWall.y);
        }
    });
}

function drawGame(roomCode) {

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    if (tableImageLoaded) {
        ctx.drawImage(tableImage, 0, 0, 855, 541);
    }

    walls.forEach(wall => wall.render(ctx));
    player1.render(ctx);
    player2.render(ctx);
    puck.render(ctx);

    ctx.font = "20px Arial";
    ctx.fillStyle = "black";
    ctx.textAlign = "center";
    ctx.fillText(`${player1.nickname} (${player1.score}) - ${player2.nickname} (${player2.score})`, canvas.width / 2, canvas.height - 30);

    ctx.fillText(`Room: ${roomCode}`, canvas.width / 2, canvas.height - 10);

    if (scoreMessage) {
        ctx.font = "30px Arial";
        ctx.fillStyle = "red";
        ctx.fillText(scoreMessage, canvas.width / 2, canvas.height / 2);
    }
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

export { drawGame, updatePlayerPosition, updatePuckPosition, addWall, updateWallPosition };
