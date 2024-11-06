using System;

namespace AirHockey.Observers
{
    public interface ISubject
    {
        void Attach(IObserver observer);

        void Detach(IObserver observer);

        void Notify(int scorer);
    }
}
