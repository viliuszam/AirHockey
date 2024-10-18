namespace AirHockey.Actors.Walls.WallBuilder
{
    public class StaticWallBuilder : IWallBuilder
    {
        private int _id;
        private float _width;
        private float _height;
        private string? _type;
        private float _X;
        private float _Y;
        private float _Acceleration;
        private float _Mass;
        private readonly string[] _allowedTypes = { "Teleporting", "QuickSand", "Standard", "Bouncy" };

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
            if (!_allowedTypes.Contains(type))
            {
                throw new ArgumentException($"Invalid wall type: {type}.");
            }
            _type = type;
            return this;
        }

        public IWallBuilder SetPosition(float x, float y)
        {
            _X = x;
            _Y = y;
            return this;
        }

        public IWallBuilder SetVelocity(float velocityX = 0f, float velocityY = 0f)
        {
            return this;
        }

        public IWallBuilder SetAcceleration()
        {
            _Acceleration = 0f;
            return this;
        }

        public IWallBuilder SetMass()
        {
            switch (_type)
            {
                case "Teleporting":
                case "QuickSand":
                    _Mass = 0f;
                    break;
                case "Standard":
                case "Bouncy":
                    _Mass = 500f;
                    break;
            }
            return this;
        }

        public Wall Build()
        {
            Wall wall;
            switch (_type)
            {
                case "Teleporting":
                    wall = new TeleportingWall(_id, _width, _height);

                    break;
                case "QuickSand":
                    wall = new QuickSandWall(_id, _width, _height);

                    break;
                case "Standard":
                    wall = new StandardWall(_id, _width, _height, false);

                    break;
                case "Bouncy":
                    wall = new BouncyWall(_id, _width, _height, false);

                    break;
                default:
                    throw new ArgumentException("Invalid wall type");
            }
            wall.X = _X;
            wall.Y = _Y;
            wall.Acceleration = _Acceleration;
            wall.Mass = _Mass;

            return wall;
        }
    }
}