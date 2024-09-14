export class Player {
    constructor(id, color, startX, startY, nickname) {
        this.id = id;
        this.color = color;
        this.x = startX;
        this.y = startY;
        this.nickname = nickname;
        this.score = 0;
    }

    move(x, y) {
        this.x = x;
        this.y = y;
    }

    render(ctx) {
        ctx.beginPath();
        ctx.arc(this.x, this.y, 20, 0, Math.PI * 2);
        ctx.fillStyle = this.color;
        ctx.fill();
        ctx.closePath();

        ctx.font = "16px Arial";
        ctx.fillStyle = "black";
        ctx.textAlign = "center";
        ctx.fillText(`${this.nickname} (${this.score})`, this.x, this.y - 30);
    }
}