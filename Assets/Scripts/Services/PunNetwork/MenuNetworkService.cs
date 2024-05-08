using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utils;

namespace Services.PunNetwork
{
    public class MenuNetworkService : MonoBehaviourPunCallbacks, IMenuNetworkService
    {
        private byte _maxPlayersPerRoom = 4;
        private string _gameVersion = "1";
        bool isConnecting;
        
        void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            if (!PhotonNetwork.IsConnected)
            {
                print("Connecting to server..");
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void Connect()
        {
            isConnecting = true;

            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Joining Room...");
                PhotonNetwork.JoinRandomRoom();
            }
            else
                Debug.Log(("Still connecting..."));
        }

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
            {
                Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room." +
                          "\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("JoinedRoom");
            
            if (PhotonNetwork.CurrentRoom.PlayerCount == _maxPlayersPerRoom && PhotonNetwork.IsMasterClient)
            {
                Debug.Log("We load the Game scene");
                PhotonNetwork.LoadLevel(Enumerators.SceneName.Game.ToString());
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == _maxPlayersPerRoom && PhotonNetwork.IsMasterClient)
            {
                Debug.Log("We load the Game scene");
                PhotonNetwork.LoadLevel(Enumerators.SceneName.Game.ToString());
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Create a new Room");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = _maxPlayersPerRoom});
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("OnDisconnected:  " + cause);
            Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

            // #Critical: we failed to connect or got disconnected. There is not much we can do. Typically, a UI system should be in place to let the user attemp to connect again.

            isConnecting = false;

        }
    }
}