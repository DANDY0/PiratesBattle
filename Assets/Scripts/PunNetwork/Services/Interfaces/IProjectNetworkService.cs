using System;

namespace PunNetwork.Services.Impls
{
    public interface IProjectNetworkService
    {
        bool IsGameStarted { get; set; }
        event Action ConnectedToMasterEvent;
    }
}