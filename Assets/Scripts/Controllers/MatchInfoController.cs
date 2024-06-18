using Core.Abstracts;
using PunNetwork.Services;
using PunNetwork.Services.GameNetwork;
using UniRx;
using UnityEngine.UI;
using Views;

namespace Controllers
{
    public class MatchInfoController : Controller<MatchInfoView>
    {
        private readonly IGameNetworkService _gameNetworkService;
        private readonly Button _leaveButton;

        public MatchInfoController
        (
            IGameNetworkService gameNetworkService
        )
        {
            _gameNetworkService = gameNetworkService;
        }

        public override void Initialize()
        {
            View.LeaveButton.OnClickAsObservable().Subscribe(_ => OnLeaveButton()).AddTo(View);
        }

        public void Show()
        {
            View.Reset();
            View.Show();
        }

        public void SetWinnerInfo(string winner, int score, float timer) => 
            View.SetWinnerInfo(winner, score, timer);

        private void OnLeaveButton() => 
            _gameNetworkService.LeaveGameplay();
    }
}