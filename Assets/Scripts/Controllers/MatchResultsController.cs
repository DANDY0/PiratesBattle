using System.Collections;
using Behaviours;
using Core.Abstracts;
using DG.Tweening;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PunNetwork.Services.GameNetwork;
using Utils;
using Views;
using static Photon.PhotonUnityNetworking.Code.Common.Enumerators;


namespace Controllers
{
    public class MatchResultsController : Controller<MatchResultsView>
    {
        private readonly LoadingController _loadingController;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IGameNetworkService _gameNetworkService;

        public MatchResultsController
        (
            LoadingController loadingController,
            ICoroutineRunner coroutineRunner,
            IGameNetworkService gameNetworkService
        )
        {
            _loadingController = loadingController;
            _coroutineRunner = coroutineRunner;
            _gameNetworkService = gameNetworkService;
        }
        
        public void Show(GameResult gameResult)
        {
            View.Reset();
            View.Show();
            View.PlayAnimation(gameResult).OnComplete(() => _gameNetworkService.LeaveGame());
        }
        
    }
}