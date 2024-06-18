using System;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PunNetwork.Services.CustomProperties;
using PunNetwork.Services.ObjectsInRoom;
using PunNetwork.Services.SpawnPoints;
using Services.SceneLoading;
using States;
using States.Core;
using UnityEngine;
using Utils;
using Zenject;
using static Utils.Enumerators;

namespace PunNetwork.Services.GameNetwork
{
    public class GameNetworkService : MonoBehaviourPunCallbacks, IGameNetworkService, IInitializable, IDisposable
    {
        private ISceneLoadingService _sceneLoadingService;
        private ICustomPropertiesService _customPropertiesService;
        private IObjectsInRoomService _objectsInRoomService;
        private ISpawnPointsService _spawnPointsService;
        private IGameStateMachine _gameStateMachine;


        [Inject]
        private void Construct
        (
            ISceneLoadingService sceneLoadingService,
            ICustomPropertiesService customPropertiesService,
            IObjectsInRoomService objectsInRoomService,
            ISpawnPointsService spawnPointsService,
            IGameStateMachine stateMachine
        )
        {
            _sceneLoadingService = sceneLoadingService;
            _customPropertiesService = customPropertiesService;
            _objectsInRoomService = objectsInRoomService;
            _spawnPointsService = spawnPointsService;
            _gameStateMachine = stateMachine;
        }

        public void Initialize()
        {
            _customPropertiesService.PlayerLivesChangedEvent += OnPlayerLivesChanged;
            PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        }
        
        public void Dispose()
        {
            _customPropertiesService.PlayerLivesChangedEvent -= OnPlayerLivesChanged;
            PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
        }

        public void Setup()
        {
            if (!PhotonNetwork.IsConnected)
            {
                _sceneLoadingService.LoadScene(SceneNames.Menu);
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

            CheckEndOfGame();

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}",
                    PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>

        public void LeaveGameplay()
        {
            _gameStateMachine.Enter<GameResultsState>();
        }
        
        private void OnEventReceived(EventData photonEvent)
        {
            if (photonEvent.Code == GameEventCodes.StartMatchEventCode) 
                _gameStateMachine.Enter<GameplayState>();
        }
        
        private void OnPlayerLivesChanged()
        {
            _objectsInRoomService.UpdateHearts();
            CheckEndOfGame();
        }

        private void SpawnPlayer()
        {
            PhotonNetwork.Instantiate(GameObjectEntryKey.Player.ToString(),
                _spawnPointsService.GetPlayerPosition(PhotonNetwork.LocalPlayer.ActorNumber - 1, 
                    PhotonNetwork.LocalPlayer.GetPhotonTeam()), Quaternion.identity);
        }

        private void CheckEndOfGame()
        {
            if (!_objectsInRoomService.IsAllEnemiesDestroyed()) 
                return;

            if (PhotonNetwork.IsMasterClient)
                StopAllCoroutines();

            var winner = "";
            var score = -1;

            foreach (var p in PhotonNetwork.PlayerList)
            {
                if (p.GetScore() <= score) continue;
                winner = p.NickName;
                score = p.GetScore();
            }

            StartCoroutine(EndOfGame(winner, score));
        }

        private IEnumerator EndOfGame(string winner, int score)
        {
            var timer = 5.0f;

            while (timer > 0.0f)
            {
                //_gameMenuController.SetWinnerInfo(winner, score, timer);

                Debug.Log($"Player {winner} won with {score} points.\n\n\nReturning to login screen in {timer:n2} seconds.");
                yield return new WaitForEndOfFrame();

                timer -= Time.deltaTime;
            }

            PhotonNetwork.LeaveRoom();
        }
    }
}