using System;
using Core.Interfaces;
using UnityEngine;

namespace Core.Abstracts
{
    public class View : MonoBehaviour, IView
    {
        public event Action ShowEvent;
        public event Action HideEvent;

        public virtual void Show()
        {
            gameObject.SetActive(true);
            ShowEvent?.Invoke();
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            HideEvent?.Invoke();
        }
    }
}