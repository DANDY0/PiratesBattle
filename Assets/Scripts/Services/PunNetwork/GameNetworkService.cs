using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utils;

namespace Services.PunNetwork
{
    public class GameNetworkService : MonoBehaviourPunCallbacks, IGameNetworkService
    {
        [SerializeField] private Vector3[] _spawnPoints;

        private PlayerView.TeamCreator _teamCreator;
        private PlayerSpawner _playerSpawner;

        private void Awake()
        {
            _teamCreator = new PlayerView.TeamCreator();
            _playerSpawner = new PlayerSpawner(_teamCreator, _spawnPoints);

            /*
            var spawnPointsLeft = Resources.Load<GameObject>("Level/LeftSpawnPoints");
            var spawnPointsRight = Resources.Load<GameObject>("Level/RightSpawnPoints");
            List<Transform>  pointsLeft =  new List<Transform>();
            List<Transform>  pointsRight =  new List<Transform>();

            foreach (Transform point in spawnPointsLeft.transform)
                pointsLeft.Add(point);
            foreach (Transform point in spawnPointsRight.transform)
                pointsRight.Add(point);*/

            // _spawnPointsLeft = pointsLeft;
            // _spawnPointsRight = pointsRight;
        }

        void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(Enumerators.SceneName.Menu.ToString());
                return;
            }

            if (PhotonNetwork.InRoom)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                // PhotonNetwork.Instantiate(this._playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                _teamCreator.AssignTeam();
                _playerSpawner.SpawnPlayer();
                // SpawnPlayer();
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

                LoadArena();
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

                LoadArena();
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(Enumerators.SceneName.Menu.ToString());
        }

        public void LeaveGameplay()
        {
            PhotonNetwork.LeaveRoom();
        }


        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }

            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

            PhotonNetwork.LoadLevel("PunBasics-Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }

        public void QuitApplication()
        {
            Application.Quit();
        }
    }


    public class PlayerSpawner
    {
        private readonly Vector3[] _spawnPoints;

        private PlayerView.TeamCreator _teamCreator;

        public PlayerSpawner(PlayerView.TeamCreator teamCreator, Vector3[] spawnPoints)
        {
            _teamCreator = teamCreator;
            _spawnPoints = spawnPoints;
        }

        public void SpawnPlayer()
        {
            PhotonNetwork.Instantiate("TeamPlayers\\" + Enumerators.TeamType.MyPlayer, Vector3.zero,
                Quaternion.identity);
        }
    }
}