using Controllers;
using Services.Window;
using UnityEngine;
using Utils.Extensions;
using Views;
using Zenject;

namespace Installers.Game
{
    
    [CreateAssetMenu(menuName = "Installers/GamePrefabInstaller", fileName = "GamePrefabInstaller")]
    public class GamePrefabInstaller : ScriptableObjectInstaller
    {
        [Header("Canvas")] 
        [SerializeField] private Canvas _canvas;

        [Header("Windows")] 
        [SerializeField] private MainView _mainView;

        public override void InstallBindings()
        {
            BindWindows();
        }
        
        private void BindWindows()
        {
            Container.Resolve<IWindowService>().ClearWindows();
            
            var canvas = Container.InstantiatePrefabForComponent<Canvas>(_canvas);
            Container.Bind<Canvas>().FromInstance(canvas).AsSingle();
            
            var parent = canvas.transform;
            
            Container.AddWindowToQueue<MainController, MainView>(_mainView, parent, 0, isFocusable: true);
            
            Container.BindWindows();
        }
    }
}