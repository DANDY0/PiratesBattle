using System;
using Photon.Pun;
using PunNetwork.Services.ProjectNetwork;
using Services.Data;
using States.Core;
using Utils;

namespace States
{
    public class LoadDataState : IState, IDisposable
    {
        private readonly IDataService _dataService;
        private readonly IProjectNetworkService _projectNetworkService;

        public LoadDataState
        (
            IDataService dataService,
            IProjectNetworkService projectNetworkService
        )
        {
            _dataService = dataService;
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
                PhotonNetwork.LoadLevel(SceneNames.Game);
            }
            else
                PhotonNetwork.LoadLevel(SceneNames.Menu);
        }
    }
}