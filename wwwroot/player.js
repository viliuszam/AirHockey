class Player {
    constructor(id, color, startX, startY) {
        this.id = id;
        this.color = color;
        this.x = startX;
        this.y = startY;
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
    }
}

export { Player };