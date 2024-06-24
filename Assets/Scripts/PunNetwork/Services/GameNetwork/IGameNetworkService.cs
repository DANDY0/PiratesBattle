using Utils;
using static Utils.Enumerators;

namespace PunNetwork.Services.GameNetwork
{
    public interface IGameNetworkService
    {
        void Setup();
        GameResult GameResult { get; }
        void LeaveGame();
    }
}