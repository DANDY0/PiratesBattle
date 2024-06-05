using System;

namespace PunNetwork.Services.Impls
{
    public interface IInitialNetworkService
    {
        event Action ConnectedToMasterEvent;
    }
}