using System;
using System.Collections;
using System.Linq;
using Controllers;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.ObjectsInRoom;
using PunNetwork.Services.SpawnPoints;
using States;
using States.Core;
using UnityEngine;
using Utils;
using Utils.Extensions;
using Zenject;
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

        private const float MaxHealthPoints = 100;
        
        public GameResult GameResult { get; private set; }

        [Inject]
        private void Construct
        (
            ICustomPropertiesService customPropertiesService,
            IObjectsInRoomService objectsInRoomService,
            ISpawnPointsService spawnPointsService,
            IGameStateMachine stateMachine,
            LoadingController loadingController
        )
        {
            _customPropertiesService = customPropertiesService;
            _objectsInRoomService = objectsInRoomService;
            _spawnPointsService = spawnPointsService;
            _gameStateMachine = stateMachine;
            _loadingController = loadingController;
        }

        public void Initialize()
        {
            _customPropertiesService.PlayerHealthPointsChangedEvent += OnPlayerHealthPointsChanged;
            PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        }

        public void Dispose()
        {
            _customPropertiesService.PlayerHealthPointsChangedEvent -= OnPlayerHealthPointsChanged;
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
                SpawnPlayer();
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
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
            var playerPosition = _spawnPointsService.GetPlayerPosition(PhotonNetwork.LocalPlayer.ActorNumber - 1,
                photonTeam);
            PhotonNetwork.Instantiate(GameObjectEntryKey.Player.ToString(), playerPosition, Quaternion.identity);
        }

        private void CheckIfGameEnded()
        {
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

            if (!allSameTeam) return;

            var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            var sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(GameEventCodes.EndMatchEventCode, firstPlayerTeam, raiseEventOptions, sendOptions);
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