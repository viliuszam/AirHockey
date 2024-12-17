namespace AirHockey.Achievement
{
    public interface IAchievementElement
    {
        void Accept(IAchievementVisitor visitor);
    }

}