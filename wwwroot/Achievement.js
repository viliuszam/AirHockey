export class Achievement {
    constructor(message) {
        this.message = message;
        this.y = -15; 
        this.opacity = 0;
        this.duration = 3000; 
        this.startTime = Date.now();
    }

    update(elapsedTime) {
        const fadeInDuration = 500; 
        const fadeOutDuration = 500; 
        const holdTime = this.duration - fadeInDuration - fadeOutDuration;

        if (elapsedTime < fadeInDuration) {
            this.opacity = elapsedTime / fadeInDuration;
            this.y = -50 + (50 * (elapsedTime / fadeInDuration));
        } else if (elapsedTime < fadeInDuration + holdTime) {
            this.opacity = 1;
            this.y = 0;
        } else if (elapsedTime < this.duration) {
            const fadeOutProgress = (elapsedTime - fadeInDuration - holdTime) / fadeOutDuration;
            this.opacity = 1 - fadeOutProgress;
            this.y = 0 - (50 * fadeOutProgress);
        } else {
            return false;
        }

        return true;
    }

    render(ctx, yOffset, trophyIcon) {
        ctx.save();
        ctx.globalAlpha = this.opacity;

        const trophyWidth = 30; 
        const padding = 20; 
        ctx.font = "20px Arial";
        const textWidth = ctx.measureText(this.message).width;

        const rectWidth = trophyWidth + padding + textWidth + padding; 
        const rectHeight = 50; 

        const rectX = (ctx.canvas.width - rectWidth) / 2; 
        const rectY = 15 + yOffset + this.y;

        ctx.fillStyle = "rgba(50, 50, 50, 0.7)";
        ctx.fillRect(rectX, rectY, rectWidth, rectHeight);
        ctx.strokeStyle = "gold";
        ctx.lineWidth = 3;
        ctx.strokeRect(rectX, rectY, rectWidth, rectHeight);

        // Draw trophy icon
        const iconX = rectX + 10; 
        const iconY = rectY + (rectHeight - trophyWidth) / 2; 
        if (trophyIcon.complete) {
            ctx.drawImage(trophyIcon, iconX, iconY, trophyWidth, trophyWidth);
        }

        ctx.textAlign = "left";

        const textX = iconX + trophyWidth + padding; 
        const textY = rectY + rectHeight / 2 + 7; 

        ctx.strokeStyle = "black";
        ctx.lineWidth = 3;
        ctx.strokeText(this.message, textX, textY);

        ctx.fillStyle = "gold";
        ctx.fillText(this.message, textX, textY);

        ctx.restore();
    }
}
