using AirHockey.Ambience.Effects;

namespace AirHockey.Ambience
{
    public interface IIterator<T> : IEnumerable<T>
    {
        bool HasNext();
        T Next();
        T First();
        void Add(T item);
        void Remove(T item);
    }
}
