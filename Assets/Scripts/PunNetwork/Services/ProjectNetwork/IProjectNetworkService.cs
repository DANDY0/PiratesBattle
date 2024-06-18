using System;

namespace PunNetwork.Services.ProjectNetwork
{
    public interface IProjectNetworkService
    {
        bool IsGameStarted { get; set; }
        event Action ConnectedToMasterEvent;
    }
}