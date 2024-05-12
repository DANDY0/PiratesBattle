using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Services.PunNetwork.Impls;
using Services.SceneLoading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utils;
using Zenject;

namespace Services.PunNetwork
{
    public class GameNetworkService : MonoBehaviourPunCallbacks, IGameNetworkService
    {
        private ISceneLoadingService _sceneLoadingService;
        private IPlayerNetworkService _playerNetworkService;
        private ICustomPropertiesService _customPropertiesService;
        
        [SerializeField] private Vector3[] _spawnPoints;

        private PlayerView.TeamCreator _teamCreator;
        private PlayerSpawner _playerSpawner;
        private LoadBalancingClient _lbc;

    
        [Inject]
        private void Construct
        (
            ISceneLoadingService sceneLoadingService,
            IPlayerNetworkService playerNetworkService,
            ICustomPropertiesService customPropertiesService
        )
        {
            _sceneLoadingService = sceneLoadingService;
            _playerNetworkService = playerNetworkService;
            _customPropertiesService = customPropertiesService;
        }

        private void Awake()
        {
            _lbc = new LoadBalancingClient();
            // _lbc.AddCallbackTarget(_customPropertiesService);
            // _lbc.AddCallbackTarget(_playerNetworkService);
        }

        void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(SceneNames.Menu);
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

        void Update()
        {
            // "back" button of phone equals "Escape". quit app if that's pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitApplication();
            }
        }


        public override void OnJoinedRoom()
        {
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
        
        public void QuitApplication()
        {
            Application.Quit();
        }
    }


}