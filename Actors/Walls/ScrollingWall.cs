namespace AirHockey.Actors.Walls
{
    public class ScrollingWall : Wall
    {
        private const float scrollSpeed = 0.01f;
        private string direction = "UP";
        private int currentIter = 0;
        private int iterCount = 1000;

        public int GetIter()
        {
            return this.iterCount;
        }

        public string GetDirection()
        {
            return this.direction;
        }

        public ScrollingWall(int id, float width, float height)
            : base(id, width, height)
        {
        }

        public override void Update(){
            MoveUpDown();
            ApplyFriction();
            Move();
        }

        public void MoveUpDown(){
            if(direction == "UP"){
                VelocityY += scrollSpeed;
                currentIter++;
                if(currentIter == iterCount){
                    direction = "DOWN";
                    currentIter = 0;
                }
            }
            else if (direction == "DOWN"){
                VelocityY -= scrollSpeed;
                currentIter++;
                if(currentIter == iterCount){
                    direction = "UP";
                    currentIter = 0;
                }
            }
        }
    }
}