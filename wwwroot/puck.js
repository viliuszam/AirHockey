class Puck {
    constructor(startX, startY) {
        this.x = startX;
        this.y = startY;
    }

    move(x, y) {
        this.x = x;
        this.y = y;
    }

    render(ctx) {
        ctx.beginPath();
        ctx.arc(this.x, this.y, 10, 0, Math.PI * 2);
        ctx.fillStyle = "black";
        ctx.fill();
        ctx.closePath();
    }
}

export { Puck };
