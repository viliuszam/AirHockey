using AirHockey.Observers;

namespace AirHockey.Actors
{
    public class Game
    {
        public Room Room { get; private set; }
        public Puck Puck { get; set; }
        public int Player1Score { get; set; } = 0;
        public int Player2Score { get; set; } = 0;

        private List<IGoalObserver> observers = new List<IGoalObserver>();
        public bool HasObservers { get; set; } = false;
        public Game(Room room)
        {
            Room = room;
            Puck = new Puck();
        }

        public void RegisterObserver(IGoalObserver observer)
        {
            observers.Add(observer);
        }

        public void UnregisterObserver(IGoalObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers(Player scorer)
        {
            foreach (var observer in observers)
            {
                observer.OnGoalScored(scorer, this);
            }
        }

        public void GoalScored(Player scorer)
        {
            if (scorer == Room.Players[0]) //Player1
                Player1Score++;
            else
                Player2Score++;

            NotifyObservers(scorer);
        }

        public void StartGame()
        {
            Player1Score = 0;
            Player2Score = 0;
        }
    }
}
