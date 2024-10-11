namespace AirHockey.Actors.Buffs
{
    public class AccelerationBuff : IBuff
    {
        public void ApplyBuff(Player player)
        {
            player.Acceleration += 0.1f;
        }
    }
}
