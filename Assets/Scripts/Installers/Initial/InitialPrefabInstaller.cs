using System;
using Controllers;
using Services.Window;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Extensions;
using Views;
using Zenject;

namespace Installers.Initial
{
    [CreateAssetMenu(menuName = "Installers/InitialPrefabInstaller", fileName = "InitialPrefabInstaller")]
    public class InitialPrefabInstaller : ScriptableObjectInstaller
    {
        [Header("Canvas")] 
        [SerializeField] private Canvas _canvas;
        
        [Header("Windows")]
        [SerializeField] private WaitingDataView _waitingDataView;
        
        public override void InstallBindings()
        {
            DependencyInjector.Container = Container;
            BindWindows();
        }

        private void BindWindows()
        {
            Container.Resolve<IWindowService>().ClearWindows();
            var parent = Instantiate(_canvas).transform;
            
            Container.AddWindowToQueue<WaitingDataController, WaitingDataView>(_waitingDataView, parent, 0);
            
            Container.BindWindows();
        }
    }
}