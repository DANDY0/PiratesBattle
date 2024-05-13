using Controllers;
using Services.PunNetwork;
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
        [SerializeField] private GameMenuView _gameMenuView;

        [Header("Prefabs")] 
        [SerializeField] private GameNetworkService _gameNetworkService;
        [SerializeField] private CustomPropertiesService _customPropertiesService;


        public override void InstallBindings()
        {
            BindWindows();
            BindPrefabs();
        }

        private void BindPrefabs()
        {
            Container.BindPrefab(_gameNetworkService);
            Container.BindPrefab(_customPropertiesService);
        }
        
        private void BindWindows()
        {
            Container.Resolve<IWindowService>().ClearWindows();
            
            var canvas = Container.InstantiatePrefabForComponent<Canvas>(_canvas);
            Container.Bind<Canvas>().FromInstance(canvas).AsSingle();
            
            var parent = canvas.transform;
            
            Container.AddWindowToQueue<MainController, MainView>(_mainView, parent, 0, isFocusable: true);
            Container.AddWindowToQueue<GameMenuController, GameMenuView>(_gameMenuView, parent, 1);

            Container.BindWindows();
        }
    }
}