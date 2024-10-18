namespace AirHockey.Actors.Walls.WallBuilder
{
    public class DynamicWallBuilder : IWallBuilder
    {
        private int _id;
        private float _width;
        private float _height;
        private string? _type;
        private float _X;
        private float _Y;
        private float _VelocityX;
        private float _VelocityY;
        private float _Acceleration;
        private float _Mass;

        private readonly string[] _allowedTypes = { "Bouncy", "Scrolling" };

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
            _VelocityX = velocityX;
            _VelocityY = velocityY;
            return this;
        }
        
        public IWallBuilder SetAcceleration()
        {
            switch (_type)
            {
                case "Bouncy":
                    _Acceleration = 0.95f;
                    break;
                case "Scrolling":
                    _Acceleration = 0.75f;
                    break;
            }
            return this;
        }

        public IWallBuilder SetMass()
        {
            switch (_type)
            {
                case "Bouncy":
                    _Mass = 0.1f;
                    break;
                case "Scrolling":
                    _Mass = 1.0f;
                    break;
            }
            return this;
        }

        public Wall Build()
        {
            Wall wall;
            switch (_type)
            {
                case "Bouncy":
                    wall = new BouncyWall(_id, _width, _height, true);
                    break;
                case "Scrolling":
                    wall = new ScrollingWall(_id, _width, _height);
                    break;
                default:
                    throw new ArgumentException("Invalid wall type");
            }
            wall.X = _X;
            wall.Y = _Y;
            wall.VelocityX = _VelocityX;
            wall.VelocityY = _VelocityY;
            wall.Acceleration = _Acceleration;
            wall.Mass = _Mass;

            return wall;
        }
    }
}