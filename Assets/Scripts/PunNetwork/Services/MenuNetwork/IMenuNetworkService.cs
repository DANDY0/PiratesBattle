    using System;

    namespace PunNetwork.Services.MenuNetwork
{
    public interface IMenuNetworkService
    {
        public event Action RoomFilledEvent;
        void Connect();
        void SetMaxPlayers(byte count);
    }
}