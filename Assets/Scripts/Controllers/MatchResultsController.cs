using Core.Abstracts;
using DG.Tweening;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Services.SceneLoading;
using Utils;
using Views;

namespace Controllers
{
    public class MatchResultsController : Controller<MatchResultsView>
    {
        private readonly LoadingController _loadingController;
        private readonly ISceneLoadingService _sceneLoadingService;

        public MatchResultsController
        (
            LoadingController loadingController,
            ISceneLoadingService sceneLoadingService
        )
        {
            _loadingController = loadingController;
            _sceneLoadingService = sceneLoadingService;
        }
        
        public void Show()
        {
            View.Reset();
            View.Show();
            View.PlayAnimation().OnComplete(() =>
            {
                PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
                PhotonNetwork.LeaveRoom();
                _sceneLoadingService.LoadScene(SceneNames.Menu);
                _loadingController.Show();
            });
        }
    }
}