using System;
using AirHockey.Actors.Walls;

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

        public Wall Build()
        {
            if (!_type.HasValue)
            {
                throw new InvalidOperationException("Wall type must be set before building the wall.");
            }

            Wall wall = _type.Value switch
            {
                StaticWallType.Teleporting => new TeleportingWall(_id, _width, _height),
                StaticWallType.QuickSand => new QuickSandWall(_id, _width, _height),
                StaticWallType.Standard => new StandardWall(_id, _width, _height, false),
                StaticWallType.Bouncy => new BouncyWall(_id, _width, _height, false),
                StaticWallType.Undo => new UndoWall(_id, _width, _height),
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