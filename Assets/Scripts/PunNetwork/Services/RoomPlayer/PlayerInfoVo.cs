using PunNetwork.NetworkData;
using PunNetwork.Views.Player;
using static Utils.Enumerators;


namespace Models
{
    public class PlayerInfoVo
    {
        public PlayerView View;
        public bool IsLocalPlayersSpawned;
        public bool IsLocalPoolsPrepared;
        public NetworkDataModel.PlayerImmutableDataVo ImmutableDataVo;
        public TeamRole TeamRole;
    }
}