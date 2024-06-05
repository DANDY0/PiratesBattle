using System;
using Photon.Pun;
using PunNetwork.Services.Impls;
using Services.Data;
using Services.SceneLoading;
using States.Core;
using Utils;

namespace States
{
    public class LoadDataState : IState, IDisposable
    {
        private readonly IDataService _dataService;
        private readonly ISceneLoadingService _sceneLoadingService;
        private readonly IProjectNetworkService _projectNetworkService;

        public LoadDataState
        (
            IDataService dataService,
            ISceneLoadingService sceneLoadingService,
            IProjectNetworkService projectNetworkService
        )
        {
            _dataService = dataService;
            _sceneLoadingService = sceneLoadingService;
            _projectNetworkService = projectNetworkService;
        }
        
        public void Enter()
        {
            _dataService.DataLoadedEvent += OnDataLoaded;
            _dataService.StartLoading();
        }

        public void Exit()
        {
        }

        public void Dispose()
        {
            _dataService.DataLoadedEvent -= OnDataLoaded;
        }

        private void OnDataLoaded()
        {
            _projectNetworkService.IsGameStarted = PhotonNetwork.InRoom;
            
            if (_projectNetworkService.IsGameStarted)
            {
                PhotonNetwork.RejoinRoom(PhotonNetwork.CurrentRoom.Name);
                _sceneLoadingService.LoadScene(SceneNames.Game);
            }
            else
                _sceneLoadingService.LoadScene(SceneNames.Menu);
        }
    }
}