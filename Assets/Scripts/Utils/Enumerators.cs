namespace Utils
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
            PlayerSpawnedData,
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
            Player = 1
        }

        public enum GameResult
        {
            Win = 0,
            Lose = 1,
            Draw = 2
        }    }
}