using Controllers;
using Services.PunNetwork;
using Services.Window;
using UnityEngine;
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
        [SerializeField] private PunNetworkService _punNetworkService;

        //[SerializeField] private SettingsView _settingsView;

        public override void InstallBindings()
        {
            BindWindows();
            BindPrefabs();
        }

        private void BindPrefabs()
        {
            Container.BindPrefab(_punNetworkService);
        }

        private void BindWindows()
        {
            Container.Resolve<IWindowService>().ClearWindows();
            var parent = Instantiate(_canvas).transform;
            
            //Container.AddWindowToQueue<SettingsController, SettingsView>(_settingsView, parent, 1);
            Container.AddWindowToQueue<MenuSettingsController, MenuSettingsView>(_menuSettingsView, parent, 0);
            
            
            Container.BindWindows();
        }
    }
}