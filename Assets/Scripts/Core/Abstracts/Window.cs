using Core.Interfaces;
using Enums;

namespace Core.Abstracts
{
    public abstract class Window : View, IWindow
    {
        public abstract EWindow Name { get; }

        public virtual void Open() => Show();

        public virtual void Close() => Hide();
    }
}