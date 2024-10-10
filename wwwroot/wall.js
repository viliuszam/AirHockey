export class Wall {
    constructor(wallId, x, y, width, height, wallType) {
        this.wallId = wallId;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.wallType = wallType;
    }

    move(x, y) {
        this.x = x;
        this.y = y;
    }

    render(ctx) {
        switch (this.wallType) {
            case 'QuickSandWall':
                const patternCanvas = document.createElement('canvas');
                patternCanvas.width = 10;
                patternCanvas.height = 10;
                const patternCtx = patternCanvas.getContext('2d');
                patternCtx.fillStyle = 'rgba(128, 128, 128, 0.75)'; 
                patternCtx.beginPath();
                patternCtx.arc(5, 5, 2, 0, 2 * Math.PI);
                patternCtx.fill();

                const pattern = ctx.createPattern(patternCanvas, 'repeat');
                ctx.fillStyle = pattern;
                ctx.fillRect(this.x, this.y, this.width, this.height);
                break;

            case 'StandardWall':
                ctx.fillStyle = 'black';
                ctx.fillRect(this.x, this.y, this.width, this.height);
                break;
            case 'BouncyWall':
                ctx.fillStyle = 'green';
                ctx.fillRect(this.x, this.y, this.width, this.height);
                break;
            case 'TeleportingWall':
                ctx.fillStyle = 'purple';
                ctx.fillRect(this.x, this.y, this.width, this.height);
                break;
            case 'ScrollingWall':
                ctx.fillStyle = 'blue';
                ctx.fillRect(this.x, this.y, this.width, this.height);
                break;
            default:
                ctx.fillStyle = 'gray';
                ctx.fillRect(this.x, this.y, this.width, this.height);
                break;
        }
    }
}