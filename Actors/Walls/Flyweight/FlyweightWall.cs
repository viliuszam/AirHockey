namespace AirHockey.Actors.Walls.Flyweight
{
    public class FlyweightWall
    {
        public float Width { get; private set; }
        public float Height { get; private set; }
        public string Type { get; private set; }

        public FlyweightWall(float width, float height, string type)
        {
            Width = width;
            Height = height;
            Type = type;
        }
    }
}
