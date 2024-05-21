using Core.Abstracts;
using PunNetwork.Services;
using UniRx;
using UnityEngine.UI;
using Views;
using Zenject;

namespace Controllers
{
    public class GameMenuController : Controller<GameMenuView>, IInitializable
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
            View.Reset();
            View.LeaveButton.OnClickAsObservable().Subscribe(_ => Connect()).AddTo(View);
        }

        public void SetWinnerInfo(string winner, int score, float timer) => 
            View.SetWinnerInfo(winner, score, timer);
        
        private void Connect() => 
            _gameNetworkService.LeaveGameplay();
    }
}