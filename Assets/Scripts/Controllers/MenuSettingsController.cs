using Core.Abstracts;
using Databases;
using Enums;
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

        public MenuSettingsController
        (
            ISceneLoadingService sceneLoadingService,
            IWindowService windowService,
            ISettingsDatabase settingsDatabase
        )
        {
            _sceneLoadingService = sceneLoadingService;
            _windowService = windowService;
            _settingsDatabase = settingsDatabase;
        }

        public void Initialize()
        {
            View.Show();
            View.PlayButton.OnClickAsObservable().Subscribe(_ => _sceneLoadingService.LoadScene(SceneNames.Game))
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
    }
}