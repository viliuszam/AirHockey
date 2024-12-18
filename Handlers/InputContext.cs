using AirHockey.Actors;
using AirHockey.Services;

namespace AirHockey.Handlers;

public class InputContext
{
    public string RoomCode { get; set; }
    public string ConnectionId { get; set; }
    public Dictionary<string, bool> Inputs { get; set; }
    public Game Game { get; set; }
    public Player Player { get; set; }
    public GameService Service { get; set; }
}
