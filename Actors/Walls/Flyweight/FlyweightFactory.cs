namespace AirHockey.Actors.Walls.Flyweight
{
    public class FlyweightFactory
    {
        private Dictionary<string, FlyweightWall> _flyweightWalls = new Dictionary<string, FlyweightWall>();

        public FlyweightWall GetFlyweightWall(float width, float height, string type)
        {
            string key = $"{width}-{height}-{type}";
            if (!_flyweightWalls.ContainsKey(key))
            {
                _flyweightWalls[key] = new FlyweightWall(width, height, type);
            }
            return _flyweightWalls[key];
        }
    }
}
