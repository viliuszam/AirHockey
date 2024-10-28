using System.Diagnostics.Eventing.Reader;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AirHockey.Actors.Walls
{
    public class UndoWall : Wall
    {
        private bool active = true;
        private int currentIter = 500;
        private int iterCount = 500;

        public int GetIter()
        {
            return this.iterCount;
        }

        public UndoWall(int id, float width, float height)
            : base(id, width, height)
        {
        }

        public override void Update(){
            if(!active){
                if(currentIter >= iterCount){
                    active = true;
                }
                else{
                    currentIter++;
                }
            }
        }

        public bool isActive(){
            return active;
        }

        public void setInactive(){
            active = false;
            currentIter = 0;
        }
    }
}