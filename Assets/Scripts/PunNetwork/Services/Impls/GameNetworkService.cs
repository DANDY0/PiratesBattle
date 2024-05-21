using System.Collections;
using Controllers;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Services.SceneLoading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using static Utils.Enumerators;
using Utils.Extensions;
using Zenject;

namespace PunNetwork.Services.Impls
{
    public class GameNetworkService : MonoBehaviourPunCallbacks, IGameNetworkService
    {
        private ISceneLoadingService _sceneLoadingService;
        private IPlayerNetworkService _playerNetworkService;
        private ILoadBalancingClient _loadBalancingClient;
        private ICustomPropertiesService _customPropertiesService;
        private IPlayersInRoomService _playersInRoomService;


        [Inject]
        private void Construct
        (
            ISceneLoadingService sceneLoadingService,
            IPlayerNetworkService playerNetworkService,
            ILoadBalancingClient loadBalancingClient,
            ICustomPropertiesService customPropertiesService,
            IPlayersInRoomService playersInRoomService
            //GameMenuController gameMenuController
        )
        {
            _sceneLoadingService = sceneLoadingService;
            _playerNetworkService = playerNetworkService;
            _loadBalancingClient = loadBalancingClient;
            _customPropertiesService = customPropertiesService;
            _playersInRoomService = playersInRoomService;
            //_gameMenuController = gameMenuController;
        }

        private void Start()
        {
            _loadBalancingClient.AddCallbackTarget(_customPropertiesService);
            _customPropertiesService.PlayerLivesChangedEvent += OnPlayerLivesChanged;
            if (!PhotonNetwork.IsConnected)
            {
                _sceneLoadingService.LoadScene(SceneNames.Menu);
                return;
            }

            if (PhotonNetwork.InRoom)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                _playerNetworkService.SpawnOurPlayer();
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }

        private void OnDestroy()
        {
            _loadBalancingClient.AddCallbackTarget(_customPropertiesService);
            _customPropertiesService.PlayerLivesChangedEvent += OnPlayerLivesChanged;
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
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}",
                    PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            _sceneLoadingService.LoadScene(SceneNames.Menu);
        }

        public void LeaveGameplay()
        {
            PhotonNetwork.LocalPlayer.LeaveCurrentTeam();
            PhotonNetwork.LeaveRoom();
        }

        private void OnPlayerLivesChanged()
        {
            _playersInRoomService.UpdateHearts();
            CheckEndOfGame();
        }

        private void CheckEndOfGame()
        {
            if (!_playersInRoomService.IsAllEnemiesDestroyed()) 
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