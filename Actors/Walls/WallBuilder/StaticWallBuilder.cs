using System;
using AirHockey.Actors.Walls;
using AirHockey.Actors.Walls.Flyweight;

namespace AirHockey.Actors.Walls.WallBuilder
{
    public enum StaticWallType
    {
        Teleporting,
        QuickSand,
        Standard,
        Bouncy,
        Undo
    }

    public class StaticWallBuilder : IWallBuilder
    {
        private int _id;
        private float _width;
        private float _height;
        private StaticWallType? _type;
        private float _X;
        private float _Y;
        private float _Acceleration;
        private float _Mass;
        private bool _movable;  

        private FlyweightFactory _flyweightFactory; 

        public StaticWallBuilder(FlyweightFactory flyweightFactory)
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

        public IWallBuilder SetType(string type)
        {
            if (Enum.TryParse(type, true, out StaticWallType parsedType))
            {
                _type = parsedType;
                return this;
            }
            throw new ArgumentException($"Invalid wall type: {type}");
        }

        public IWallBuilder SetPosition(float x, float y)
        {
            _X = x;
            _Y = y;
            return this;
        }

        public IWallBuilder SetVelocity(float velocityX = 0f, float velocityY = 0f)
        {
            // Static walls do not use velocity, so this method does nothing
            return this;
        }

        public IWallBuilder SetAcceleration()
        {
            _Acceleration = 0f;
            return this;
        }

        public IWallBuilder SetMass()
        {
            if (_type.HasValue)
            {
                _Mass = _type == StaticWallType.Standard || _type == StaticWallType.Bouncy ? 500f : 0f;
            }
            return this;
        }

        public IWallBuilder SetMovable(bool movable)  // New method to set the movable flag
        {
            _movable = movable;
            return this;
        }

        public Wall Build()
        {
            if (!_type.HasValue)
            {
                throw new InvalidOperationException("Wall type must be set before building the wall.");
            }

            FlyweightWall flyweight = _flyweightFactory.GetFlyweightWall(_width, _height, _type.Value.ToString());

            Wall wall = _type.Value switch
            {
                StaticWallType.Teleporting => new TeleportingWall(_id, flyweight),
                StaticWallType.QuickSand => new QuickSandWall(_id, flyweight),
                StaticWallType.Standard => new StandardWall(_id, flyweight, _movable),
                StaticWallType.Bouncy => new BouncyWall(_id, flyweight, _movable),
                StaticWallType.Undo => new UndoWall(_id, flyweight),
                _ => throw new ArgumentException("Invalid wall type")
            };

            wall.X = _X;
            wall.Y = _Y;
            wall.Acceleration = _Acceleration;
            wall.Mass = _Mass;

            return wall;
        }
    }
}
