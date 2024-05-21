using System;
using Utils;

namespace PunNetwork.Services
{
    public interface ICustomPropertiesService
    {
        public event Action PlayerSpawnedEvent;
        event Action PlayerLivesChangedEvent;
    }
}