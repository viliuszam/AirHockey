export class Powerup {
    constructor(powerupId, x, y, powerupType, isActive) {
        this.powerupId = powerupId;
        this.x = x;
        this.y = y;
        this.powerupType = powerupType;
        this.isActive = isActive;
    }

    render(ctx) {
        if (this.isActive) {
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
                case 'PushPowerup':
                    ctx.beginPath();
                    ctx.arc(this.x, this.y, 20, 0, Math.PI * 2);
                    ctx.fillStyle = 'cyan';
                    ctx.fill();
                    ctx.closePath();
                    break;
                default:
                    break;
            }
        }
    }
}