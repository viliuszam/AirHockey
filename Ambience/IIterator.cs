namespace AirHockey.Ambience
{
    public interface IIterator<T>
    {
        bool HasNext();
        T Next();
        T First();
        void Add(T item);
        void Remove(T item);
    }
}
