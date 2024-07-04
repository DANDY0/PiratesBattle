using System;

namespace PunNetwork.MasterEvent
{
    public interface IMasterEventService
    {
        void Setup();
        void OnAllDataGet();
        void OnAllPlayersSpawned();
        void OnAllPoolsPrepared();
        void RaiseEvent(byte eventCode, object eventContent = null);
        void Subscribe(byte eventCode, Action<object> handler);
        void Unsubscribe(byte eventCode, Action<object> handler);
    }
}