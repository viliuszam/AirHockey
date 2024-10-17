using AirHockey.Actors.Walls;
namespace AirHockey.Actors.Walls.WallBuilder
{
    public interface IWallBuilder
    {
        IWallBuilder SetId(int id);
        IWallBuilder SetDimensions(float width, float height);
        IWallBuilder SetPosition(float x, float y);
        IWallBuilder SetType(string type);
        IWallBuilder SetVelocity(float velocityX = 0f, float velocityY = 0f);
        IWallBuilder SetAcceleration();
        IWallBuilder SetMass();
        Wall Build();
    }
}