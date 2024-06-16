using System;
using Photon.Pun;
using Zenject;

namespace PunNetwork.Services.Impls
{
    public class InitialNetworkService : MonoBehaviourPunCallbacks, IInitialNetworkService
    {
        public event Action ConnectedToMasterEvent;

        public override void OnConnectedToMaster() => ConnectedToMasterEvent?.Invoke();
    }
}