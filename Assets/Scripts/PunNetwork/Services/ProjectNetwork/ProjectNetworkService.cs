using System;
using Photon.Pun;

namespace PunNetwork.Services.ProjectNetwork
{
    public class ProjectNetworkService : MonoBehaviourPunCallbacks, IProjectNetworkService
    {
        public bool IsGameStarted { get; set; }
        
        public event Action ConnectedToMasterEvent;

        public override void OnConnectedToMaster() => ConnectedToMasterEvent?.Invoke();
    }
}