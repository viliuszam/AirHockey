using AirHockey.Actors;

namespace AirHockey.Mediators
{
    public interface IMediator
    {
        void SendMessage(string roomCode, string playerId, string message);
        void RegisterPlayer(Player player);
        void RemovePlayer(Player player);
    }

}
