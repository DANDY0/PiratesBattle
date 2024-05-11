using Core.Abstracts;
using Databases;
using Enums;
using Services.PunNetwork;
using Services.PunNetwork.Impls;
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
            View.PlayButton.OnClickAsObservable().Subscribe(_ => Connect())
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

        private void Connect()
        {
            _menuNetworkService.Connect();
        }
        
    }
}