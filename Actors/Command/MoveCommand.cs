namespace AirHockey.Actors.Command
{
    public class MoveCommand : ICommand
    {
        private Entity _entity;
        private Queue<(float X, float Y, float timestamp)> _positionHistory;
        private const float TimeWindow = 5.0f;

        public MoveCommand(Entity entity, Queue<(float X, float Y, float timestamp)> positionHistory)
        {
            _entity = entity;
            _positionHistory = positionHistory;
        }

        public Entity getEntity(){
            return _entity;
        }

        public void Execute()
        {
            float currentTime = GetCurrentTime();
            _positionHistory.Enqueue((_entity.X, _entity.Y, currentTime));
        }

        public void Undo()
        {
            float currentTime = GetCurrentTime();
            (float lastX, float lastY) = (-1, -1);

            while (_positionHistory.Count > 0 && _positionHistory.Peek().timestamp < currentTime - TimeWindow)
            {
                var oldestPosition = _positionHistory.Dequeue();
                lastX = oldestPosition.X;
                lastY = oldestPosition.Y;
            }

            if (_positionHistory.Count == 0 && lastX != -1 && lastY != -1)
            {
                _entity.X = lastX;
                _entity.Y = lastY;
            }
            else if (_positionHistory.Count > 0)
            {
                var targetPosition = _positionHistory.Peek();
                _entity.X = targetPosition.X;
                _entity.Y = targetPosition.Y;
            }
        }

        private float GetCurrentTime()
        {
            return DateTime.Now.Second + DateTime.Now.Millisecond / 1000.0f;
        }
    }
}