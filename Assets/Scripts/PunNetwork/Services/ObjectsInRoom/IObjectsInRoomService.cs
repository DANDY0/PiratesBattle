using System.Collections.Generic;
using Photon.Realtime;
using PunNetwork.Views.Player;

namespace PunNetwork.Services.ObjectsInRoom
{
    public interface IObjectsInRoomService
    {
        List<PlayerView> PlayerViews { get; }
        void OnPlayerSpawned(Player player, PlayerView playerView);
        void UpdateHealthPoints(Player player, float newHealthPoints);
        void PlayerLeftRoom(Player player);
    }
}