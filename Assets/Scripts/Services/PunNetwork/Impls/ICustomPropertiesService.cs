using System;
using Utils;

namespace Services.PunNetwork.Impls
{
    public interface ICustomPropertiesService
    {
        public event Action PlayersSpawnedEvent;
        public void SetPlayerProperty(Enumerators.PlayerProperty property, object value);
    }
}