using System;
using System.Collections.Generic;
using Core.Abstracts;
using Core.Interfaces;
using Models;
using Services.Window;
using UnityEngine;
using Zenject;
using IPrefabProvider = Photon.PhotonUnityNetworking.Code.Common.PrefabProvider.IPrefabProvider;
using Object = UnityEngine.Object;
using PrefabProvider = Photon.PhotonUnityNetworking.Code.Common.PrefabProvider.PrefabProvider;

namespace Utils.Extensions
{
    public static class BindExtensions
    {
        private static readonly Queue<WindowBindingInfoVo> WindowQueue = new();
        public static int WindowsCount;

        public static void BindView<T, TU>(this DiContainer container, Object viewPrefab)
            where TU : IView
            where T : IController
        {
            container.BindInterfacesAndSelfTo<T>().AsSingle();
            var instance = container.InstantiatePrefabForComponent<TU>(viewPrefab);
            container.Bind<TU>().FromInstance(instance).AsSingle();
        }

        public static void BindViewDisabled<T, TU>(this DiContainer container, Object viewPrefab)
            where TU : IView
            where T : IController
        {
            container.BindInterfacesAndSelfTo<T>().AsSingle();
            var instance = container.InstantiatePrefabForComponent<TU>(viewPrefab);
            container.Bind<TU>()
                .FromInstance(instance)
                .AsSingle()
                .OnInstantiated((_, o) => (o as MonoBehaviour)?.gameObject.SetActive(false));
        }

        public static void AddWindowToQueue<T, TU>(this DiContainer container, Object viewPrefab, Transform parent,
            int orderNumber, bool isFocusable = false, bool isDontDestroyOnLoad = false)
            where TU : IWindow where T : IController
        {
            container.BindInterfacesAndSelfTo<T>().AsSingle();
            container.AddWindowToQueue<TU>(viewPrefab, parent, orderNumber, isFocusable: isFocusable,
                isDontDestroyOnLoad: isDontDestroyOnLoad);
        }

        public static void AddWindowToQueue<T>(this DiContainer container, Object viewPrefab, Transform parent,
            int orderNumber, bool isFocusable = false, bool isDontDestroyOnLoad = false) where T : IWindow
        {
            WindowsCount++;
            var windowInfo = new WindowBindingInfoVo
            {
                Type = typeof(T),
                ViewPrefab = viewPrefab,
                Parent = parent,
                OrderNumber = orderNumber,
                IsFocusable = isFocusable,
                IsDontDestroyOnLoad = isDontDestroyOnLoad
            };
            WindowQueue.Enqueue(windowInfo);
        }

        public static void BindWindows(this DiContainer container)
        {
            while (WindowQueue.Count != 0)
            {
                var windowBindingInfoVo = WindowQueue.Dequeue();

                var binding = container.BindInterfacesAndSelfTo(windowBindingInfoVo.Type)
                    .FromComponentInNewPrefab(windowBindingInfoVo.ViewPrefab)
                    .UnderTransform(windowBindingInfoVo.Parent)
                    .AsSingle();

                var index = 0;
                binding.OnInstantiated((_, instance) =>
                {
                    index++;
                    var window = instance as Window;
                    if (window == null)
                        throw new Exception(
                            $"[{nameof(BindExtensions)}] Cannot convert {windowBindingInfoVo.ViewPrefab} to window");

                    window.gameObject.SetActive(false);
                    var windowService = container.Resolve<IWindowService>();
                    windowService.RegisterWindow(window, windowBindingInfoVo.IsFocusable,
                        windowBindingInfoVo.OrderNumber, windowBindingInfoVo.IsDontDestroyOnLoad);

                    if (index == WindowsCount)
                        container.Resolve<IWindowService>().SortBySiblingIndex();
                });
            }
        }

        public static void BindPrefab<TContent>(this DiContainer container, TContent prefab, Transform parent = null,
            bool isDestroyOnLoad = false)
            where TContent : Object =>
            container.BindInterfacesTo<TContent>()
                .FromComponentInNewPrefab(prefab)
#if UNITY_EDITOR
                .UnderTransform(parent)
#endif
                .AsSingle()
                .OnInstantiated((_, o) =>
                {
                    if (isDestroyOnLoad)
                        Object.DontDestroyOnLoad(o as MonoBehaviour);
                });
        
        public static void BindPrefabs(this DiContainer container, IEnumerable<GameObjectEntry> entries)
        {
            var entriesDictionary = new Dictionary<string, GameObject>();

            foreach (var entry in entries)
            {
                if (entriesDictionary.TryGetValue(entry.Key.ToString(), out var gameObject)) 
                    Debug.LogError($"{nameof(BindExtensions)} Duplicate key {entry.Key} on {gameObject} and {entry.GameObject}");
                entriesDictionary[entry.Key.ToString()] = entry.GameObject;
            }

            container.Bind<IPrefabProvider>().To<PrefabProvider>().AsSingle().WithArguments(entriesDictionary);
        }
    }
}