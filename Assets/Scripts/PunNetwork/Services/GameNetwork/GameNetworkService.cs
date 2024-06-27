using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.ObjectsInRoom;
using PunNetwork.Services.SpawnPoints;
using Services.Data;
using States;
using States.Core;
using UnityEngine;
using Utils;
using Utils.Extensions;
using Zenject;
using static Photon.PhotonUnityNetworking.Code.Common.Enumerators;
using static PunNetwork.NetworkData.NetworkDataModel;
using static Utils.Enumerators;

namespace PunNetwork.Services.GameNetwork
{
    public class GameNetworkService : MonoBehaviourPunCallbacks, IGameNetworkService, IDisposable, IInitializable
    {
        private ICustomPropertiesService _customPropertiesService;
        private IObjectsInRoomService _objectsInRoomService;
        private ISpawnPointsService _spawnPointsService;
        private IGameStateMachine _gameStateMachine;
        private LoadingController _loadingController;
        private IDataService _dataService;

        private const float MaxHealthPoints = 100;
        
        public GameResult GameResult { get; private set; }

        private List<ReadyPlayerInfo> _readyPlayersData = new List<ReadyPlayerInfo>();
        private bool _isMatchEnded;

        [Inject]
        private void Construct
        (
            ICustomPropertiesService customPropertiesService,
            IObjectsInRoomService objectsInRoomService,
            ISpawnPointsService spawnPointsService,
            IDataService dataService,
            IGameStateMachine stateMachine,
            LoadingController loadingController
        )
        {
            _customPropertiesService = customPropertiesService;
            _objectsInRoomService = objectsInRoomService;
            _spawnPointsService = spawnPointsService;
            _dataService = dataService;
            _gameStateMachine = stateMachine;
            _loadingController = loadingController;
        }

        public void Initialize()
        {
            _customPropertiesService.PlayerHealthPointsChangedEvent += OnPlayerHealthPointsChanged;
            _customPropertiesService.GetReadyPlayerInfoEvent +=  GetReadyPlayerInfoHandler;
            PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        }

        public void Dispose()
        {
            _customPropertiesService.PlayerHealthPointsChangedEvent -= OnPlayerHealthPointsChanged;
            _customPropertiesService.GetReadyPlayerInfoEvent -=  GetReadyPlayerInfoHandler;
            PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
        }

        public void Setup()
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LoadLevel(SceneNames.Menu);
                return;
            }

            if (PhotonNetwork.InRoom)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                SendReadyPlayerInfo();
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }

        private void GetReadyPlayerInfoHandler(Player player, ReadyPlayerInfo data)
        {
            CheckIsAllPlayersReady(data);
        }

        private void CheckIsAllPlayersReady(ReadyPlayerInfo data)
        {
            _readyPlayersData.Add(data);
            
            if(!PhotonNetwork.IsMasterClient)
                return;
            
            if (_readyPlayersData.Count == PhotonNetwork.CurrentRoom.PlayerCount) 
                GameEventsRaiser.RaiseEvent(GameEventCodes.AllPlayersReady, null);
        }

        private void SendReadyPlayerInfo()
        {
            var playerSpawnedData = new ReadyPlayerInfo(PhotonNetwork.LocalPlayer.ActorNumber,
                _dataService.CachedUserLocalData.NickName, _dataService.CachedUserLocalData.SelectedCharacter.ToString(),1);
            var json = JsonConvert.SerializeObject(playerSpawnedData);

            PhotonNetwork.LocalPlayer.SetCustomProperty(PlayerProperty.ReadyPlayerInfo, json);
        }

        public void LeaveGame() => StartCoroutine(HandleGameEnd());

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}",
                    PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            }
        }

        /// <summary>
        /// Called when a Photon Player got disconnected. We need to load a smaller scene.
        /// </summary>
        /// <param name="other">Other.</param>
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

            _objectsInRoomService.PlayerLeftRoom(other);
            CheckIfGameEnded();

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}",
                    PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        private void OnEventReceived(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case GameEventCodes.StartMatchEventCode:
                    _gameStateMachine.Enter<GameplayState>();
                    break;
                case GameEventCodes.EndMatchEventCode:
                {
                    var winningTeam = (byte)photonEvent.CustomData;
                    GameResult = winningTeam == PhotonNetwork.LocalPlayer.GetPhotonTeam().Code ? GameResult.Win : GameResult.Lose;
                    _gameStateMachine.Enter<GameResultsState>();
                    break;
                }
                case GameEventCodes.AllPlayersReady:
                {
                    SpawnPlayer();
                    break;
                }
            }
        }

        private void OnPlayerHealthPointsChanged(Player player, float newHealthPoints)
        {
            _objectsInRoomService.UpdateHealthPoints(player, newHealthPoints);
            if (newHealthPoints != MaxHealthPoints)
                CheckIfGameEnded();
        }

        private void SpawnPlayer()
        {
            var photonTeam = PhotonNetwork.LocalPlayer.GetPhotonTeam();
            var playerPosition = _spawnPointsService.GetPlayerPosition(PhotonNetwork.LocalPlayer.ActorNumber - 1, photonTeam);
            
            PhotonNetwork.LocalPlayer.TryGetCustomProperty<ReadyPlayerInfo>(PlayerProperty.ReadyPlayerInfo, out var info);
            PhotonNetwork.Instantiate(info.CharacterName, playerPosition, Quaternion.identity);
        }

        private void CheckIfGameEnded()
        {
            if(_isMatchEnded)
                return;
            
            if (!PhotonNetwork.IsMasterClient)
                return;

            var alivePlayers = _objectsInRoomService.PlayerViews
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

            yield return new WaitForSeconds(0.5f);

            PhotonNetwork.LoadLevel(SceneNames.Menu);

            _loadingController.Show();
        }
    }
}