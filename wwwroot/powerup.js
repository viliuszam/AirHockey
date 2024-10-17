export class Powerup {
    constructor(powerupId, x, y, powerupType) {
        this.powerupId = powerupId;
        this.x = x;
        this.y = y;
        this.powerupType = powerupType;
    }

    render(ctx) {
        switch (this.powerupType) {
            case 'DashPowerup':
                ctx.beginPath();
                ctx.arc(this.x, this.y, 20, 0, Math.PI * 2);
                ctx.fillStyle = 'green';
                ctx.fill();
                ctx.closePath();
                break;
            case 'FreezePowerup':
                ctx.beginPath();
                ctx.arc(this.x, this.y, 20, 0, Math.PI * 2);
                ctx.fillStyle = 'purple';
                ctx.fill();
                ctx.closePath();
                break;
            default:
                break;
        }
    }
}