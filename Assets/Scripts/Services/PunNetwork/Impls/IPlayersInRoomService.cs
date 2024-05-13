namespace Services.PunNetwork.Impls
{
    public interface IPlayersInRoomService
    {
        void OnAllSpawned();
        bool IsAllReady();
    }
}