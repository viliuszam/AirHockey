export class Player {
    constructor(id, color, startX, startY, nickname) {
        this.id = id;
        this.color = color;
        this.x = startX;
        this.y = startY;
        this.nickname = nickname;
        this.score = 0;
        this.angleFacing = 0;
        this.activePowerup = "";
    }

    move(x, y) {
        this.x = x;
        this.y = y;
    }

    setAngleFacing(dx, dy) {
        if (dx !== 0 || dy !== 0) {
            this.angleFacing = Math.atan2(dy, dx);
        }
    }

    drawArrow(ctx) {
        const arrowLength = 30; // Length of the arrow
        const arrowWidth = 10; // Width of the arrowhead

        // Calculate arrowhead points
        const arrowX = this.x + Math.cos(this.angleFacing) * arrowLength;
        const arrowY = this.y + Math.sin(this.angleFacing) * arrowLength;

        ctx.beginPath();
        ctx.moveTo(this.x, this.y); // Start point of the arrow
        ctx.lineTo(arrowX, arrowY); // Main line of the arrow

        // Draw the arrowhead
        ctx.lineTo(arrowX - arrowWidth * Math.cos(this.angleFacing - Math.PI / 6),
            arrowY - arrowWidth * Math.sin(this.angleFacing - Math.PI / 6));
        ctx.moveTo(arrowX, arrowY); // Move back to the main line
        ctx.lineTo(arrowX - arrowWidth * Math.cos(this.angleFacing + Math.PI / 6),
            arrowY - arrowWidth * Math.sin(this.angleFacing + Math.PI / 6));

        ctx.strokeStyle = this.color; // Set color of the arrow
        ctx.lineWidth = 2; // Set line width
        ctx.stroke(); // Draw the arrow
        ctx.closePath();
    }

    render(ctx) {
        ctx.beginPath();
        ctx.arc(this.x, this.y, 20, 0, Math.PI * 2);
        ctx.fillStyle = this.color;
        ctx.fill();
        ctx.closePath();

        this.drawArrow(ctx);

        ctx.font = "16px Arial";
        ctx.fillStyle = "black";
        ctx.textAlign = "center";
        ctx.fillText(`${this.nickname} (${this.score})`, this.x, this.y - 30);

        // Draw powerup if active
        if (this.activePowerup) {
            // Draw background for better visibility
            const powerupText = `⚡ ${this.activePowerup}`;
            const textMetrics = ctx.measureText(powerupText);
            const padding = 5;

            ctx.fillStyle = "rgba(255, 255, 255, 0.7)";
            ctx.fillRect(
                this.x - textMetrics.width / 2 - padding,
                this.y - 55 - padding,
                textMetrics.width + padding * 2,
                22
            );

            // Draw powerup text
            ctx.fillStyle = "#FF6B00";
            ctx.fillText(powerupText, this.x, this.y - 55);
        }
    }
}