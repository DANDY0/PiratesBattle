using Core.Abstracts;
using PunNetwork.Services;
using PunNetwork.Services.MenuNetwork;
using Services.Data;
using UniRx;
using Views;
using UnityEngine;
using Views.MainMenuView;

namespace Controllers
{
    public class MainMenuController : Controller<MainMenuView>
    {
        private readonly IMenuNetworkService _menuNetworkService;
        private readonly IDataService _dataService;
        private readonly SelectedCharacterHandler _selectedCharacterHandler;
        private readonly MenuProfileHandler _menuProfileHandler;

        public MainMenuController
        (
            IMenuNetworkService menuNetworkService,
            IDataService dataService,
            SelectedCharacterHandler selectedCharacterHandler,
            MenuProfileHandler menuProfileHandler
        )
        {
            _menuNetworkService = menuNetworkService;
            _dataService = dataService;
            _selectedCharacterHandler = selectedCharacterHandler;
            _menuProfileHandler = menuProfileHandler;
        }

        public override void Initialize()
        {
            View.PlayButton1.OnClickAsObservable().Subscribe(_ => ConnectOne()).AddTo(View);
            View.PlayButton2.OnClickAsObservable().Subscribe(_ => ConnectTwo()).AddTo(View);

            View.QuitButton.OnClickAsObservable().Subscribe(_ =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            }).AddTo(View);
            
            _selectedCharacterHandler.Setup(View.SelectedCharacterPanel);
            _menuProfileHandler.Setup(View.MenuProfilePanel);
        }

        public void Setup()
        {
            View.Show();
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