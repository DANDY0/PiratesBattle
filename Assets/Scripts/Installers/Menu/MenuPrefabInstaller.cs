using Controllers;
using Photon.Pun.UtilityScripts;
using Services.PunNetwork;
using Services.Window;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private MenuSettingsView _menuSettingsView;

        [Header("PUN")]
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
            
            Container.AddWindowToQueue<MenuSettingsController, MenuSettingsView>(_menuSettingsView, parent, 0);

            
            Container.BindWindows();
        }
    }
}