using Core.Abstracts;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Services.PunNetwork;
using Services.PunNetwork.Impls;
using UnityEngine.UI;
using Views;
using Zenject;

namespace Controllers
{
    public class GameMenuController: Controller<GameMenuView>, IInitializable
    {
        private readonly IGameNetworkService _gameNetworkService;
        private readonly Button _leaveButton;

        public GameMenuController
        (
            IGameNetworkService gameNetworkService
        )
        {
            _gameNetworkService = gameNetworkService;
        }

        public void Initialize()
        {
            View.Show();
            View.LeaveButton.onClick.AddListener(Connect);
        }

        private void Connect()
        {
            _gameNetworkService.LeaveGameplay();
        }

    }
}