using Controllers;
using Photon.Pun.UtilityScripts;
using PunNetwork.Services.Impls;
using Services.Window;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Utils.Extensions;
using Views;
using Zenject;

namespace Installers.Menu
{ 
    [CreateAssetMenu(menuName = "Installers/MenuPrefabInstaller", fileName = "MenuPrefabInstaller")]
    public class MenuPrefabInstaller : ScriptableObjectInstaller
    {
        [Header("Canvas")]
        [SerializeField] private Canvas _canvas;
        
        [Header("Windows")]
        [SerializeField] private MainMenuView _mainMenuView;

        [Header("Network")]
        [SerializeField] private MenuNetworkService _menuNetworkService;
        
        public override void InstallBindings()
        {
            BindWindows();
            BindPrefabs();
        }

        private void BindPrefabs()
        {
            Container.BindPrefab(_menuNetworkService);
        }

        private void BindWindows()
        {
            Container.Resolve<IWindowService>().ClearWindows();
            var parent = Instantiate(_canvas).transform;
            
            Container.AddWindowToQueue<MainMenuController, MainMenuView>(_mainMenuView, parent, 0);

            
            Container.BindWindows();
        }
    }
}