using System.Collections;
using Controllers;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.RoomPlayer;
using PunNetwork.Services.SpawnPoints;
using Services.Data;
using States.Core;
using UnityEngine;
using Utils;
using Utils.Extensions;
using Zenject;

namespace PunNetwork.Services.GameNetwork
{
    public class GameNetworkService : MonoBehaviourPunCallbacks, IGameNetworkService
    {
        private IRoomPlayerService _roomPlayerService;
        private LoadingController _loadingController;


        private bool _isMatchEnded;

        [Inject]
        private void Construct
        (
            ICustomPropertiesService customPropertiesService,
            IRoomPlayerService roomPlayerService,
            ISpawnPointsService spawnPointsService,
            IDataService dataService,
            IGameStateMachine stateMachine,
            LoadingController loadingController
        )
        {
            _roomPlayerService = roomPlayerService;
            _loadingController = loadingController;
        }
        
        public void LeaveGame() => StartCoroutine(HandleGameEnd());

        private IEnumerator HandleGameEnd()
        {
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.LocalPlayer.GetPhotonTeam() != null) 
                    PhotonNetwork.LocalPlayer.LeaveCurrentTeam();

                PhotonNetwork.LocalPlayer.ResetCustomProperties();
                PhotonNetwork.LeaveRoom();

                while (PhotonNetwork.InRoom)
                {
                    yield return null;
                }
            }

            //yield return new WaitForSeconds(0.5f);

            PhotonNetwork.LoadLevel(SceneNames.Menu);

            _loadingController.Show();
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            CheckIfGameEnded();
        }

        private void CheckIfGameEnded()
        {
            if(_isMatchEnded)
                return;
            
            if (!PhotonNetwork.IsMasterClient)
                return;

            var alivePlayers = _roomPlayerService.PlayerViews
                .Where(p => p.CurrentHealthPoints > 0)
                .Select(playerView => playerView.Player)
                .ToList();

            if (alivePlayers.Count == 0)
            {
                Debug.Log("No players are alive.");
                return;
            }

            var firstPlayerTeam = alivePlayers.First().GetPhotonTeam().Code;
            var allSameTeam = alivePlayers.All(player => player.GetPhotonTeam().Code == firstPlayerTeam);

            if (!allSameTeam) 
                return;
        
            _isMatchEnded = true;
            
            Debug.LogError($"Raise EndMatchEvent {PhotonNetwork.LocalPlayer.ActorNumber}");
            
            GameEventsRaiser.RaiseEvent(GameEventCodes.EndMatchEventCode, firstPlayerTeam);
        }
    }
}