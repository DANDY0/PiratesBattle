using System.Collections.Generic;
using Core.Interfaces;
using UnityEngine;
using Zenject;

namespace Core.Abstracts
{
    public abstract class UiCollection<TView> : MonoBehaviour, IUiCollection where TView : View
    {
        [SerializeField] private Transform _collectionRoot;
        [Inject] private IFactory<TView> _factory;

        private readonly List<TView> _items = new();

        public TView AddItem()
        {
            var item = _factory.Create();
            item.transform.SetParent(_collectionRoot, false);
            item.Show();
            _items.Add(item);
            return item;
        }

        public List<TView> GetItems() => _items;

        public void RemoveItem(TView view)
        {
            _items.Remove(view);
            DestroyImmediate(view.gameObject);
        }

        public void Clear()
        {
            foreach (var item in _items)
                Destroy(item.gameObject);
            _items.Clear();
        }

        public int Count() => _items.Count;
    }
}