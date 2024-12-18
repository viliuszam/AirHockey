using AirHockey.Actors.Walls.Flyweight;

namespace AirHockey.Actors.Walls
{
    public class UndoWall : Wall
    {
        private bool active = true;
        private int currentIter = 500;
        private int iterCount = 500;

        // Updated constructor: Accept FlyweightWall and pass it to the base Wall constructor
        public UndoWall(int id, FlyweightWall flyweight)
            : base(id, flyweight)  // Pass the flyweight to the Wall constructor
        {
        }

        public override void Update()
        {
            if (!active)
            {
                if (currentIter >= iterCount)
                {
                    active = true;
                }
                else
                {
                    currentIter++;
                }
            }
        }

        public bool isActive()
        {
            return active;
        }

        public void setInactive()
        {
            active = false;
            currentIter = 0;
        }
    }
}
