using UnityEngine;

namespace Core.Abstracts
{
    public abstract class Handler<T> where T : View
    {
        protected T View;

        public void Setup(T view, T controller)
        {
            View = view;
            Controller = controller;
            Initialize();
        }

        public void Show() => View.Show();
        public void Hide() => View.Hide();

        protected abstract void Initialize();
    }
}