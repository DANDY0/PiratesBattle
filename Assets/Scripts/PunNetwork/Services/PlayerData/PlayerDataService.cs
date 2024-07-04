using PunNetwork.NetworkData;
using PunNetwork.Services.RoomPlayer;
using Services.Data;

namespace PunNetwork.Services.PlayerData
{
    public class PlayerDataService : IPlayerDataService
    {
        private readonly IDataService _dataService;
        private readonly IRoomPlayerService _roomPlayerService;

        public PlayerDataService
        (
            IDataService dataService,
            IRoomPlayerService roomPlayerService
        )
        {
            _dataService = dataService;
            _roomPlayerService = roomPlayerService;
        }

        public void SendImmutableData()
        {
            var playerImmutableDataVo = new NetworkDataModel.PlayerImmutableDataVo
            {
                Nickname = _dataService.CachedUserLocalData.NickName,
                CharacterName = _dataService.CachedUserLocalData.SelectedCharacter.ToString(),
                AvatarID = 1,
                InitialStats = new StatsValuesVo
                {
                    HealthPoints = 100,
                    
                }
            };

            _roomPlayerService.SendPlayerImmutableData(playerImmutableDataVo);
        }
    }
}