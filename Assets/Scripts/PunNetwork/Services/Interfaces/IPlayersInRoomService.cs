namespace PunNetwork.Services
{
    public interface IPlayersInRoomService
    {
        void OnAllSpawned();
        bool IsAllReady();
        bool IsAllEnemiesDestroyed();
        void UpdateHearts();
    }
}