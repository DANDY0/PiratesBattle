using System.Collections.Generic;
using Core.Interfaces;
using ScriptsPhotonCommon.Factory;
using UnityEngine;
using Zenject;

namespace Core.Abstracts
{
    public abstract class UiCollection<TView> : MonoBehaviour, IUiCollection, IInitializable where TView : View
    {
        [SerializeField] private Transform _collectionRoot;
        
        private readonly DiContainer _diContainer;
        private readonly List<TView> _items = new();
        private readonly IGameFactory _factory;
        private TView _view;

        protected UiCollection
        (
            IGameFactory factory,
            DiContainer diContainer
        )
        {
            _factory = factory;
            _diContainer = diContainer;
        }

        public void Initialize()
        {
            _view = _diContainer.Resolve<TView>();
        }

        public TView AddItem()
        {
            var item = _factory.Create(_view, Vector3.zero, Quaternion.identity);
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