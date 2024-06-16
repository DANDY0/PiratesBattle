using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using Utils;
using Zenject;

namespace PunNetwork.Services.Impls
{
    public class MenuNetworkService : MonoBehaviourPunCallbacks, IMenuNetworkService, IInitializable, IDisposable
    {
        private byte _maxPlayersPerRoom;
        private string _gameVersion = "1";
        private bool _isConnecting;
        private IPhotonTeamsManager _photonTeamsManager;


        [Inject]
        private void Construct
        (
            IPhotonTeamsManager photonTeamsManager
        )
        {
            _photonTeamsManager = photonTeamsManager;
        }

        public void Initialize()
        {
            _photonTeamsManager.PlayerJoinedTeam += PlayerJoinedTeam;
            _photonTeamsManager.PlayerLeftTeam += PlayerLeftTeam;
        }

        public void Dispose()
        {
            _photonTeamsManager.PlayerJoinedTeam -= PlayerJoinedTeam;
            _photonTeamsManager.PlayerLeftTeam -= PlayerLeftTeam;
        }
        
        public void SetMaxPlayers(byte count) => _maxPlayersPerRoom = count;

        public void Connect()
        {
            _isConnecting = true;

            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Joining Room...");
                PhotonNetwork.JoinRandomRoom();
            }
            else
                Debug.Log("Still connecting...");
        }

        public override void OnConnectedToMaster()
        {
            if (_isConnecting)
            {
                Debug.Log(
                    "PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room." +
                    "\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                var availableTeam = _photonTeamsManager.GetAvailableTeam();
                PhotonNetwork.LocalPlayer.JoinTeam(availableTeam);
            }

            Debug.Log("JoinedRoom");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                var availableTeam = _photonTeamsManager.GetAvailableTeam();
                newPlayer.JoinTeam(availableTeam);
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Create a new Room");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = _maxPlayersPerRoom });
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("OnDisconnected:  " + cause);
            Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

            // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.
            //PhotonNetwork.LocalPlayer.LeaveCurrentTeam();

            _isConnecting = false;
        }

        private void PlayerJoinedTeam(Player player, PhotonTeam team)
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == _maxPlayersPerRoom)
            {
                Debug.Log("We load the Game scene");
                PhotonNetwork.LoadLevel(SceneNames.Game);
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }

        private void PlayerLeftTeam(Player player, PhotonTeam team)
        {
        }
    }
}