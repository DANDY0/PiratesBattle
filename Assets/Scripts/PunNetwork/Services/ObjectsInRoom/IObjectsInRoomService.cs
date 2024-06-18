using System.Collections.Generic;
using PunNetwork.Views.Player;

namespace PunNetwork.Services.ObjectsInRoom
{
    public interface IObjectsInRoomService
    {
        bool IsAllEnemiesDestroyed();
        void UpdateHearts();
        List<PlayerView> PlayerViews { get; }
    }
}