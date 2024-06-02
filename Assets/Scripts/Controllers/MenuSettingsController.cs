using Core.Abstracts;
using Databases;
using Enums;
using PunNetwork.Services;
using Services.SceneLoading;
using Services.Window;
using UniRx;
using UnityEngine;
using Utils;
using Views;
using Zenject;

namespace Controllers
{
    public class MenuSettingsController : Controller<MenuSettingsView>, IInitializable
    {
        private readonly ISceneLoadingService _sceneLoadingService;
        private readonly IWindowService _windowService;
        private readonly ISettingsDatabase _settingsDatabase;
        private readonly IMenuNetworkService _menuNetworkService;

        public MenuSettingsController
        (
            ISceneLoadingService sceneLoadingService,
            IWindowService windowService,
            ISettingsDatabase settingsDatabase,
            IMenuNetworkService menuNetworkService
        )
        {
            _sceneLoadingService = sceneLoadingService;
            _windowService = windowService;
            _settingsDatabase = settingsDatabase;
            _menuNetworkService = menuNetworkService;
        }

        public void Initialize()
        {
            View.Show();
            View.PlayButton1.OnClickAsObservable().Subscribe(_ => ConnectOne())
                .AddTo(View);
            View.PlayButton2.OnClickAsObservable().Subscribe(_ => ConnectTwo())
                .AddTo(View);
            
            View.QuitButton.OnClickAsObservable().Subscribe(_ =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            }).AddTo(View);
        }

        private void ConnectOne()
        {
            _menuNetworkService.SetMaxPlayers(1);
            _menuNetworkService.Connect();
        }
        private void ConnectTwo()
        {
            _menuNetworkService.SetMaxPlayers(2);
            _menuNetworkService.Connect();
        }
        
    }
}