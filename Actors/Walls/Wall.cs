using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using AirHockey.Actors.Walls.Flyweight;

namespace AirHockey.Actors.Walls;

public abstract class Wall : Entity
{
    public int Id { get; set; }
    public FlyweightWall Flyweight { get; private set; }

    public float Width => Flyweight.Width;
    public float Height => Flyweight.Height;
    public string Type => Flyweight.Type;

    public Wall(int id, FlyweightWall flyweight)
    {
        Id = id;
        Flyweight = flyweight;
    }

    [ExcludeFromCodeCoverage]
    public Wall() { }

    public override bool IsColliding(Entity other)
    {
        float otherX = other.X;
        float otherY = other.Y;
        float otherRadius = other.Radius;

        bool isWithinHorizontalBounds = otherX + otherRadius >= X && otherX - otherRadius <= X + Width;
        bool isWithinVerticalBounds = otherY + otherRadius >= Y && otherY - otherRadius <= Y + Height;

        return isWithinHorizontalBounds && isWithinVerticalBounds;
    }

    public bool IsColliding(Wall otherWall)
    {
        bool isHorizontalCollision = this.X < otherWall.X + otherWall.Width && this.X + this.Width > otherWall.X;
        bool isVerticalCollision = this.Y < otherWall.Y + otherWall.Height && this.Y + this.Height > otherWall.Y;

        return isHorizontalCollision && isVerticalCollision;
    }

    public bool IsColliding(Rectangle exclusionZone)
    {
        bool isHorizontalCollision = this.X < exclusionZone.X + exclusionZone.Width && this.X + this.Width > exclusionZone.X;
        bool isVerticalCollision = this.Y < exclusionZone.Y + exclusionZone.Height && this.Y + this.Height > exclusionZone.Y;

        return isHorizontalCollision && isVerticalCollision;
    }

    public override void ConstrainToBounds(float minX, float minY, float maxX, float maxY)
    {
        if (X < minX)
        {
            X = minX;
            VelocityX = -VelocityX * 0.5f;
        }
        else if (X + Width > maxX)
        {
            X = maxX - Width;
            VelocityX = -VelocityX * 0.5f;
        }

        if (Y < minY)
        {
            Y = minY;
            VelocityY = -VelocityY * 0.5f;
        }
        else if (Y + Height > maxY)
        {
            Y = maxY - Height;
            VelocityY = -VelocityY * 0.5f;
        }
    }
}
