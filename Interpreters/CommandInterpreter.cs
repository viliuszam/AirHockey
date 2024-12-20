namespace AirHockey.Interpreters;

public class CommandInterpreter
{
    public GameCommand ParseCommand(string input)
    {
        var parts = input.Split(' ');
        if (parts.Length == 0)
        {
            return null;
        }

        string command = parts[0].ToLower();
        switch (command)
        {
            case "setscore":
                if (parts.Length > 1 && int.TryParse(parts[1], out int score))
                {
                    return new SetScoreCommand(score);
                }
                break;
            
            case "setpowerup":
                if (parts.Length > 1)
                {
                    string powerupType = parts[1];
                    return new SetPowerupCommand(powerupType);
                }
                break;

            default:
                Console.WriteLine("Unknown command.");
                break;
        }
        return null;
    }
}