import { Player } from './player.js';
import { Puck } from './puck.js';
import { scoreMessage } from './game.js';
import { Wall } from './wall.js';
import { Powerup } from './powerup.js';

let canvas = document.getElementById("gameCanvas");
let ctx = canvas.getContext("2d");

export let player1 = new Player("Player1", "red", 100, 200);
export let player2 = new Player("Player2", "blue", 700, 200);
let puck = new Puck(400, 270);
let walls = [];
let powerups = [];
let effects = [];

const tableImage = new Image();
tableImage.src = "table.png";

let tableImageLoaded = false;
tableImage.onload = () => {
    tableImageLoaded = true;
};

function addWall(wallId, x, y, width, height, wallType) {
    walls.push(new Wall(wallId, x, y, width, height, wallType));
}

function addPowerup(powerupId, x, y, powerupType, isActive) {
    powerups.push(new Powerup(powerupId, x, y, powerupType, isActive));
}

export function clearWalls() {
    walls = [];
}

export function clearPowerups() {
    powerups = [];
}

function addEffect(effectType, duration, behavior, x, y, radius) {
    if (effectType === 'LocalFieldEffect') {
        effects.push(new LocalFieldEffect(effectType, duration, behavior, x, y, radius));
    } else if (effectType === 'GlobalFieldEffect') {
        effects.push(new GlobalFieldEffect(effectType, duration, behavior));
    }
}

export function clearEffects() {
    effects = [];
}

function updateWallPosition(sentWalls) {
    sentWalls.forEach(sentWall => {
        let matchingWall = walls.find(wall => wall.wallId == sentWall.id); 
        if (matchingWall) {
            matchingWall.move(sentWall.x, sentWall.y);
        }
    });
}

function updatePowerups(activePowerups) {
    clearPowerups();
    activePowerups.forEach(powerup => {
        addPowerup(
            powerup.id,
            powerup.x,
            powerup.y,
            powerup.name,
            powerup.isActive
        );
    });
}

function updatePlayerActivePowerups(playerActivePowerups) {
    player1.activePowerup = playerActivePowerups[0];
    player2.activePowerup = playerActivePowerups[1];
}


export function updateEffects(activeEffects) {
    clearEffects();
    activeEffects.forEach(effect => {
        addEffect(
            effect.effectType,
            effect.duration,
            effect.behavior,
            effect.x,
            effect.y,
            effect.radius
        );
    });
}

class LocalFieldEffect {
    constructor(effectType, duration, behavior, x, y, radius) {
        this.effectType = effectType;
        this.duration = duration;
        this.behavior = behavior;
        this.x = x;
        this.y = y;
        this.radius = radius;
    }

    render(ctx) {
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, 2 * Math.PI);

        if (this.behavior.startsWith("LOW_GRAVITY")) {
            ctx.fillStyle = 'rgba(255, 165, 0, 0.5)';
            ctx.strokeStyle = 'rgba(255, 165, 0, 1)';
        } else if (this.behavior.startsWith("WIND")) {
            ctx.fillStyle = 'rgba(135, 206, 250, 0.5)';
            ctx.strokeStyle = 'rgba(135, 206, 250, 1)';
        }

        ctx.fill();
        ctx.lineWidth = 2;
        ctx.stroke();
        ctx.closePath();

        if (this.behavior.startsWith("WIND")) {
            let windAngle = parseFloat(this.behavior.split(',')[1]);
            this.renderWindIcon(ctx, windAngle);
        }
    }

    renderWindIcon(ctx, angle) {
        ctx.save();
        ctx.translate(this.x, this.y);
        ctx.rotate(angle);

        ctx.beginPath();
        ctx.moveTo(0, 0);
        ctx.lineTo(20, 0);
        ctx.lineTo(15, -5);
        ctx.moveTo(20, 0);
        ctx.lineTo(15, 5);
        ctx.strokeStyle = 'rgba(0, 0, 0, 0.7)';
        ctx.lineWidth = 2;
        ctx.stroke();
        ctx.closePath();

        ctx.restore();
    }
}

class GlobalFieldEffect {
    constructor(effectType, duration, behavior) {
        this.effectType = effectType;
        this.duration = duration;
        this.behavior = behavior;
    }

    render(ctx) {
        if (this.behavior.startsWith("LOW_GRAVITY")) {
            ctx.fillStyle = 'rgba(255, 165, 0, 0.2)'; 
        } else if (this.behavior.startsWith("WIND")) {
            ctx.fillStyle = 'rgba(135, 206, 250, 0.2)';
        }

        ctx.fillRect(0, 0, canvas.width, canvas.height);

        if (this.behavior.startsWith("WIND")) {
            let windAngle = parseFloat(this.behavior.split(',')[1]); 
            this.renderWindIcon(ctx, windAngle);
        }
    }

    renderWindIcon(ctx, angle) {
        ctx.save();
        ctx.translate(canvas.width / 2, canvas.height / 2); 
        ctx.rotate(angle);

        ctx.beginPath();
        ctx.moveTo(0, 0);
        ctx.lineTo(50, 0); 
        ctx.lineTo(40, -10);
        ctx.moveTo(50, 0);
        ctx.lineTo(40, 10);
        ctx.strokeStyle = 'rgba(0, 0, 0, 0.7)'; 
        ctx.lineWidth = 3;
        ctx.stroke();
        ctx.closePath();

        ctx.restore();
    }
}

function drawGame(roomCode) {

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    if (tableImageLoaded) {
        ctx.drawImage(tableImage, 0, 0, 855, 541);
    }

    effects.forEach(effect => effect.render(ctx));

    walls.forEach(wall => wall.render(ctx));
    powerups.forEach(powerup => powerup.render(ctx));
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
        const dx = x - player1.x;
        const dy = y - player1.y;
        player1.setAngleFacing(dx, dy);
        player1.move(x, y);
    } else if (playerId === "Player2") {
        const dx = x - player2.x;
        const dy = y - player2.y;
        player2.setAngleFacing(dx, dy);
        player2.move(x, y);
    }
}

function updatePuckPosition(x, y) {
    puck.move(x, y);
}

export { drawGame, updatePlayerPosition, updatePuckPosition, addWall, updateWallPosition, addPowerup, updatePowerups, updatePlayerActivePowerups };
