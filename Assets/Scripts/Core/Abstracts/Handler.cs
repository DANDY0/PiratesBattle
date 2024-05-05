using UnityEngine;

namespace Core.Abstracts
{
    public abstract class Handler<T> where T : MonoBehaviour
    {
        protected T View;

        public void Setup(T view)
        {
            View = view;
            Initialize();
        }

        protected abstract void Initialize();
    }
}