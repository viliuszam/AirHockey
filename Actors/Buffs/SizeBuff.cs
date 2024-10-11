namespace AirHockey.Actors.Buffs
{
    public class SizeBuff : IBuff
    {
        public void ApplyBuff(Player player)
        {
            player.Radius += 5f;
        }
    }
}
