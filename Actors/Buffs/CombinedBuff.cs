namespace AirHockey.Actors.Buffs
{
    public class CombinedBuff : IBuff
    {
        public void ApplyBuff(Player player)
        {
            player.Radius += 5f;
            player.Acceleration += 0.2f;
        }
    }
}
