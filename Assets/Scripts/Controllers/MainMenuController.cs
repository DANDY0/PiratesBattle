using Core.Abstracts;
using PunNetwork.Services;
using UniRx;
using Views;
using UnityEngine;

namespace Controllers
{
    public class MainMenuController : Controller<MainMenuView>
    {
        private readonly IMenuNetworkService _menuNetworkService;

        public MainMenuController
        (
            IMenuNetworkService menuNetworkService
        )
        {
            _menuNetworkService = menuNetworkService;
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