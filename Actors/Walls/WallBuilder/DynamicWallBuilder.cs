using System;
using AirHockey.Actors.Walls;
using AirHockey.Actors.Walls.Flyweight;

namespace AirHockey.Actors.Walls.WallBuilder
{
    public enum WallType
    {
        Bouncy,
        Scrolling,
        Breaking
    }

    public class DynamicWallBuilder : IWallBuilder
    {
        private int _id;
        private float _width;
        private float _height;
        private WallType? _type;
        private float _x;
        private float _y;
        private float _velocityX;
        private float _velocityY;
        private float _acceleration;
        private float _mass;

        private FlyweightFactory _flyweightFactory; // FlyweightFactory to get FlyweightWall instances

        public DynamicWallBuilder(FlyweightFactory flyweightFactory)
        {
            _flyweightFactory = flyweightFactory;
        }

        public IWallBuilder SetId(int id)
        {
            _id = id;
            return this;
        }

        public IWallBuilder SetDimensions(float width, float height)
        {
            _width = width;
            _height = height;
            return this;
        }

        public IWallBuilder SetPosition(float x, float y)
        {
            _x = x;
            _y = y;
            return this;
        }

        public IWallBuilder SetType(string type)
        {
            if (Enum.TryParse(type, true, out WallType parsedType))
            {
                _type = parsedType;
                return this;
            }
            throw new ArgumentException($"Invalid wall type: {type}");
        }

        public IWallBuilder SetVelocity(float velocityX = 0f, float velocityY = 0f)
        {
            _velocityX = velocityX;
            _velocityY = velocityY;
            return this;
        }

        public IWallBuilder SetAcceleration()
        {
            if (_type.HasValue)
            {
                _acceleration = _type == WallType.Bouncy ? 0.95f : 0.75f;
            }
            return this;
        }

        public IWallBuilder SetMass()
        {
            if (_type.HasValue)
            {
                _mass = _type == WallType.Bouncy ? 0.1f : 1.0f;
            }
            return this;
        }

        public Wall Build()
        {
            if (!_type.HasValue)
            {
                throw new InvalidOperationException("Wall type must be set before building the wall.");
            }

            // Use FlyweightFactory to get FlyweightWall instance
            FlyweightWall flyweight = _flyweightFactory.GetFlyweightWall(_width, _height, _type.Value.ToString());

            Wall wall = _type.Value switch
            {
                WallType.Bouncy => new BouncyWall(_id, flyweight, true),
                WallType.Scrolling => new ScrollingWall(_id, flyweight),
                WallType.Breaking => new BreakingWall(_id, flyweight),
                _ => throw new InvalidOperationException("Invalid wall type specified.")
            };

            wall.X = _x;
            wall.Y = _y;
            wall.VelocityX = _velocityX;
            wall.VelocityY = _velocityY;
            wall.Acceleration = _acceleration;
            wall.Mass = _mass;

            return wall;
        }
    }
}
