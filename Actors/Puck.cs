using System;
using System.Collections.Generic;
using AirHockey.Observers;
using Microsoft.AspNetCore.Hosting.Server;

namespace AirHockey.Actors
{
    public class Puck : Entity, ISubject
    {
        public float MaxSpeed = 15f;
        private List<IObserver> _observers = new List<IObserver>();

        private const float GOAL_WIDTH = 25f;
        private const float GOAL_Y_MIN = 180f;
        private const float GOAL_Y_MAX = 365f;
        private const float MAX_X = 855f;

        public Puck()
        {
            Friction = 0.98f;
            X = 427.5f;
            Y = 270.5f;
            Radius = 15f;
            Mass = 0.5f;
        }

        public override void Update()
        {
            ApplyFriction();
            Move();
            CapVelocity();
            CheckForGoal();
        }

        private void CheckForGoal()
        {
            // Check if the puck is in the goal area for Player 2
            if (X <= GOAL_WIDTH && Y >= GOAL_Y_MIN && Y <= GOAL_Y_MAX)
            {
                Notify(1); // Notify observers that Player 2 scored
            }
            // Check if the puck is in the goal area for Player 1
            else if (X >= (MAX_X - GOAL_WIDTH) && Y >= GOAL_Y_MIN && Y <= GOAL_Y_MAX)
            {
                Notify(0); // Notify observers that Player 1 scored
            }
        }

        private void CapVelocity()
        {
            float currentSpeed = (float)Math.Sqrt(VelocityX * VelocityX + VelocityY * VelocityY);
            if (currentSpeed > MaxSpeed)
            {
                float scale = MaxSpeed / currentSpeed;
                VelocityX *= scale;
                VelocityY *= scale;
            }
        }

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(int scorer)
        {
            foreach (var observer in _observers)
            {
                observer.GoalScored(scorer);
            }
        }
    }
}
