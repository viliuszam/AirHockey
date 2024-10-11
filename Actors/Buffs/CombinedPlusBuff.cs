namespace AirHockey.Actors.Buffs
{
    public class CombinedPlusBuff : IBuff
    {
        public void ApplyBuff(Player player)
        {
            player.Radius += 5f;
            player.Acceleration += 0.1f;
            player.Mass += 0.4f;
        }
    }
}
