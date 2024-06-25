namespace Photon.PhotonUnityNetworking.Code.Common
{
    public class Enumerators
    {
        public enum TeamRole
        {
            MyPlayer,
            AllyPlayer,
            EnemyPlayer,
        }

        public enum PlayerProperty
        {
            IsPlayersSpawned,
            PlayerHP,
            ReadyPlayerInfo,
			CharacterName,
			IsPoolsPrepared
        }
        public enum Character
        {
            Primo,
            Voron,
            Rocket
        }
        public enum GameObjectEntryKey
        {
            Bullet = 0,
            Primo = 1,
            Voron = 2,
            Rocket = 3,
            
        }

        public enum GameResult
        {
            Win = 0,
            Lose = 1,
            Draw = 2
        }    }
}