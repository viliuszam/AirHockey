﻿using AirHockey.Achievement;
using AirHockey.Actors.Powerups;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace AirHockey.Actors
{
    public class Player : Entity, IAchievementElement
    {
        public string Id { get; set; }
        public string Color { get; set; }
        public float MaxSpeed = 4f;
        public float AngleFacing { get; set; }
        public string Nickname { get; set; }
        public Room Room { get; private set; }
        public Powerup? ActivePowerup { get; set; }

        // How far the player is from his goal when colliding with puck (for achievement)
        public float DistanceToGoalLastCollision { get; set; }

        public Player(string id, string color, float X, float Y, string nickname, Room room)
        {
            Id = id;
            Color = color;
            Acceleration = 0.5f;
            this.X = X;
            this.Y = Y;
            Radius = 20f;
            Mass = 1f;
            Room = room;
            AngleFacing = 0f;
            Nickname = nickname;
            DistanceToGoalLastCollision = 0f;
        }

        public override void Update()
        {
            Move();
            ApplyFriction();
            VelocityX = Math.Clamp(VelocityX, -MaxSpeed, MaxSpeed);
            VelocityY = Math.Clamp(VelocityY, -MaxSpeed, MaxSpeed);
        }

        public void Accelerate(float xDirection, float yDirection)
        {
            VelocityX += xDirection * Acceleration;
            VelocityY += yDirection * Acceleration;
            if (VelocityX != 0 || VelocityY != 0)
            {
                AngleFacing = (float)Math.Atan2(VelocityY, VelocityX);
            }
        }

        public void UsePowerup()
        {
            if (ActivePowerup != null)
            {
                ActivePowerup.Activate(this);
                ActivePowerup = null;
            }
        }

        public void Accept(IAchievementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

}
