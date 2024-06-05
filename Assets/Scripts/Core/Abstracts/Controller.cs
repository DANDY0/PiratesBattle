using Core.Interfaces;
using Zenject;

namespace Core.Abstracts
{
    public abstract class Controller<T> : IInitializable, IController where T : IView
    {
        [Inject] protected readonly T View;
        public virtual void Initialize()
        {}
    }
}