namespace AirHockey.Ambience
{
    public interface IAggregate<T>
    {
        IIterator<T> CreateIterator();
    }
}
