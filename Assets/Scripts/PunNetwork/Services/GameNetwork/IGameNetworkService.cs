using Utils;
using static Photon.PhotonUnityNetworking.Code.Common.Enumerators;

namespace PunNetwork.Services.GameNetwork
{
    public interface IGameNetworkService
    {
        void Setup();
        Enumerators.GameResult GameResult { get; }
        void LeaveGame();
    }
}